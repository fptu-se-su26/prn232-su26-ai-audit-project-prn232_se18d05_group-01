using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Auth;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class AuthService : IAuthService
    {
        private const string PlayerRole = "Player";
        private const string OwnerRole = "Owner";
        private const string InvalidLoginError = "Invalid email/phone or password";
        private const int VerifyEmailOtpExpiryMinutes = 10;
        private const int PasswordResetOtpExpiryMinutes = 10;
        private const int DefaultRefreshTokenExpiryDays = 30;
        private const int RefreshTokenBytes = 64;
        private const int RefreshTokenReuseGraceSeconds = 2;
        private const int RefreshTokenCleanupRetentionDays = 7;
        private const int ResendCooldownSeconds = 60;
        private const int MaxOtpFailedAttempts = 5;
        private const string InvalidRefreshTokenError = "Invalid refresh token.";
        private const string PasswordResetSentMessage = "If this email exists, a password reset code has been sent.";
        private readonly PlayCourtDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IVerificationTokenService _verificationTokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public AuthService(
            PlayCourtDbContext dbContext,
            IJwtTokenService jwtTokenService,
            IVerificationTokenService verificationTokenService,
            IEmailService emailService,
            IConfiguration? configuration = null,
            IMemoryCache? memoryCache = null)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
            _verificationTokenService = verificationTokenService;
            _emailService = emailService;
            _configuration = configuration ?? new ConfigurationBuilder().Build();
            _memoryCache = memoryCache ?? new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            var errors = ValidateRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<RegisterResponseDto>.Fail("Register failed", errors);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var normalizedPhone = request.PhoneNumber.Trim();
            var frontendRole = request.Role.Trim();
            var userRole = MapRole(frontendRole);

            if (await _dbContext.Users.AnyAsync(u => u.Email == normalizedEmail))
            {
                return ApiResponse<RegisterResponseDto>.Fail("Register failed", ["Email already exists"]);
            }

            if (await _dbContext.Users.AnyAsync(u => u.Phone == normalizedPhone))
            {
                return ApiResponse<RegisterResponseDto>.Fail("Register failed", ["Phone number already exists"]);
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var now = DateTimeOffset.Now;
            var user = new User
            {
                Email = normalizedEmail,
                Phone = normalizedPhone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = userRole,
                Status = UserStatus.Active,
                IsEmailVerified = false,
                CreatedAt = now,
                IsDeleted = false
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var userProfile = new UserProfile
            {
                UserId = user.Id,
                FullName = request.FullName.Trim(),
                CreatedAt = now
            };

            _dbContext.UserProfiles.Add(userProfile);
            await _dbContext.SaveChangesAsync();

            string? businessName = null;
            if (userRole == UserRole.CourtOwner)
            {
                businessName = request.BusinessName!.Trim();
                _dbContext.CourtOwnerProfiles.Add(new CourtOwnerProfile
                {
                    UserProfileId = userProfile.Id,
                    BusinessName = businessName,
                    VerificationStatus = CourtOwnerVerificationStatus.Pending,
                    CreatedAt = now
                });

                await _dbContext.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            var registerMessage = "Register successfully";
            try
            {
                var otp = await _verificationTokenService.CreateOtpAsync(
                    user.Id,
                    VerificationTokenPurpose.EmailVerification,
                    VerifyEmailOtpExpiryMinutes);
                await _emailService.SendVerifyEmailAsync(user.Email, otp);
            }
            catch
            {
                registerMessage = "Register successfully, but verification email could not be sent. Please request a new code.";
            }

            return ApiResponse<RegisterResponseDto>.Ok(new RegisterResponseDto
            {
                Id = user.Id,
                FullName = userProfile.FullName,
                Email = user.Email,
                PhoneNumber = user.Phone ?? string.Empty,
                Role = ToFrontendRole(user.Role),
                Status = user.Status.ToString(),
                IsEmailVerified = user.IsEmailVerified,
                BusinessName = businessName
            }, registerMessage);
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var errors = ValidateLoginRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<LoginResponseDto>.Fail("Login failed", errors);
            }

            var identifier = request.Identifier.Trim();
            var normalizedIdentifier = identifier.ToLowerInvariant();
            var user = await _dbContext.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == normalizedIdentifier || u.Phone == identifier);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<LoginResponseDto>.Fail("Login failed", [InvalidLoginError]);
            }

            if (user.Status != UserStatus.Active)
            {
                return ApiResponse<LoginResponseDto>.Fail("Login failed", ["User account is not active"]);
            }

            await CleanupOldRefreshTokensAsync(DateTimeOffset.UtcNow);

            var token = _jwtTokenService.GenerateAccessToken(user, user.UserProfile);
            var refreshToken = CreateRefreshToken(user.Id);
            _dbContext.RefreshTokens.Add(refreshToken.Entity);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
            {
                AccessToken = token.AccessToken,
                RefreshToken = refreshToken.Value,
                ExpiresAt = token.ExpiresAt,
                RefreshTokenExpiresAt = refreshToken.Entity.ExpiresAt,
                User = BuildLoginUserDto(user)
            }, "Login successfully");
        }

        public async Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var errors = ValidateRefreshTokenRequest(request.RefreshToken);
            if (errors.Count > 0)
            {
                return ApiResponse<RefreshTokenResponseDto>.Fail("Validation failed", errors);
            }

            var now = DateTimeOffset.UtcNow;
            await CleanupOldRefreshTokensAsync(now);

            var refreshTokenValue = request.RefreshToken.Trim();
            var tokenHash = HashRefreshToken(refreshTokenValue);
            var storedToken = await _dbContext.RefreshTokens
                .Include(token => token.User)
                .ThenInclude(user => user.UserProfile)
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);

            if (storedToken is null)
            {
                return ApiResponse<RefreshTokenResponseDto>.Fail(InvalidRefreshTokenError);
            }

            if (storedToken.RevokedAt is not null)
            {
                if (CanUseRotatedTokenInGracePeriod(storedToken, now))
                {
                    var cacheKey = GetRefreshTokenGraceCacheKey(storedToken.TokenHash);
                    if (_memoryCache.TryGetValue(cacheKey, out RefreshTokenResponseDto? cachedResponse)
                        && cachedResponse is not null)
                    {
                        return ApiResponse<RefreshTokenResponseDto>.Ok(
                            cachedResponse,
                            "Token refreshed successfully.");
                    }

                    return ApiResponse<RefreshTokenResponseDto>.Fail(InvalidRefreshTokenError);
                }

                if (!string.IsNullOrWhiteSpace(storedToken.ReplacedByTokenHash))
                {
                    await RevokeActiveRefreshTokensAsync(storedToken.UserId, now);
                }

                return ApiResponse<RefreshTokenResponseDto>.Fail(InvalidRefreshTokenError);
            }

            if (storedToken.ExpiresAt <= now)
            {
                storedToken.RevokedAt = now;
                storedToken.UpdatedAt = now;
                await _dbContext.SaveChangesAsync();

                return ApiResponse<RefreshTokenResponseDto>.Fail(InvalidRefreshTokenError);
            }

            if (storedToken.User.Status != UserStatus.Active)
            {
                return ApiResponse<RefreshTokenResponseDto>.Fail("User account is not active");
            }

            return await RotateRefreshTokenAsync(storedToken, now);
        }

        private async Task<ApiResponse<RefreshTokenResponseDto>> RotateRefreshTokenAsync(
            RefreshToken storedToken,
            DateTimeOffset now)
        {
            var accessToken = _jwtTokenService.GenerateAccessToken(storedToken.User, storedToken.User.UserProfile);
            var newRefreshToken = CreateRefreshToken(storedToken.UserId, now);
            storedToken.RevokedAt = now;
            storedToken.ReplacedByTokenHash = newRefreshToken.Entity.TokenHash;
            storedToken.UpdatedAt = now;

            _dbContext.RefreshTokens.Add(newRefreshToken.Entity);
            await _dbContext.SaveChangesAsync();

            var response = new RefreshTokenResponseDto
            {
                AccessToken = accessToken.AccessToken,
                RefreshToken = newRefreshToken.Value,
                ExpiresAt = accessToken.ExpiresAt,
                RefreshTokenExpiresAt = newRefreshToken.Entity.ExpiresAt
            };

            _memoryCache.Set(
                GetRefreshTokenGraceCacheKey(storedToken.TokenHash),
                response,
                TimeSpan.FromSeconds(RefreshTokenReuseGraceSeconds));

            return ApiResponse<RefreshTokenResponseDto>.Ok(response, "Token refreshed successfully.");
        }

        public async Task<ApiResponse<object>> LogoutAsync(LogoutRequestDto request)
        {
            var errors = ValidateRefreshTokenRequest(request.RefreshToken);
            if (errors.Count > 0)
            {
                return ApiResponse<object>.Fail("Validation failed", errors);
            }

            var tokenHash = HashRefreshToken(request.RefreshToken.Trim());
            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);

            if (storedToken is null)
            {
                return ApiResponse<object>.Fail(InvalidRefreshTokenError);
            }

            if (storedToken.RevokedAt is null)
            {
                var now = DateTimeOffset.UtcNow;
                storedToken.RevokedAt = now;
                storedToken.UpdatedAt = now;
                await _dbContext.SaveChangesAsync();
            }

            return ApiResponse<object>.Ok(null, "Logout successfully.");
        }

        public async Task<ApiResponse<object>> VerifyEmailAsync(VerifyEmailRequestDto request)
        {
            var errors = ValidateVerifyEmailRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<object>.Fail("Validation failed", errors);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user is null)
            {
                return ApiResponse<object>.Fail("User not found.");
            }

            if (user.IsEmailVerified)
            {
                return ApiResponse<object>.Fail("Email already verified.");
            }

            var latestToken = await GetLatestUnusedEmailVerificationTokenAsync(user.Id);
            if (latestToken is null)
            {
                return ApiResponse<object>.Fail("Invalid verification code.");
            }

            if (latestToken.ExpiresAt <= DateTimeOffset.Now)
            {
                return ApiResponse<object>.Fail("Verification code has expired.");
            }

            if (latestToken.FailedAttempts >= MaxOtpFailedAttempts)
            {
                return ApiResponse<object>.Fail("Invalid verification code.");
            }

            var isValid = await _verificationTokenService.VerifyOtpAsync(
                user.Id,
                VerificationTokenPurpose.EmailVerification,
                request.Otp);

            if (!isValid)
            {
                return ApiResponse<object>.Fail("Invalid verification code.");
            }

            user.IsEmailVerified = true;
            user.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Email verified successfully.");
        }

        public async Task<ApiResponse<object>> ResendVerifyEmailAsync(ResendVerifyEmailRequestDto request)
        {
            var errors = ValidateResendVerifyEmailRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<object>.Fail("Validation failed", errors);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user is null)
            {
                return ApiResponse<object>.Fail("User not found.");
            }

            if (user.IsEmailVerified)
            {
                return ApiResponse<object>.Fail("Email already verified.");
            }

            var now = DateTimeOffset.Now;
            var latestToken = await GetLatestUnusedEmailVerificationTokenAsync(user.Id);
            if (latestToken is not null
                && latestToken.CreatedAt.AddSeconds(ResendCooldownSeconds) > now)
            {
                return ApiResponse<object>.Fail("Please wait before requesting another code.");
            }

            var oldTokens = await _dbContext.VerificationTokens
                .Where(t => t.UserId == user.Id
                    && t.Purpose == VerificationTokenPurpose.EmailVerification
                    && t.UsedAt == null)
                .ToListAsync();

            foreach (var token in oldTokens)
            {
                token.UsedAt = now;
                token.UpdatedAt = now;
            }

            await _dbContext.SaveChangesAsync();

            var otp = await _verificationTokenService.CreateOtpAsync(
                user.Id,
                VerificationTokenPurpose.EmailVerification,
                VerifyEmailOtpExpiryMinutes);

            try
            {
                await _emailService.SendVerifyEmailAsync(user.Email, otp);
            }
            catch
            {
                return ApiResponse<object>.Fail("Verification email could not be sent.");
            }

            return ApiResponse<object>.Ok(null, "Verification code has been sent to your email.");
        }

        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var errors = ValidateForgotPasswordRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<object>.Fail("Validation failed", errors);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user is null)
            {
                return ApiResponse<object>.Ok(null, PasswordResetSentMessage);
            }

            var now = DateTimeOffset.Now;
            var oldTokens = await _dbContext.VerificationTokens
                .Where(t => t.UserId == user.Id
                    && t.Purpose == VerificationTokenPurpose.PasswordReset
                    && t.UsedAt == null)
                .ToListAsync();

            foreach (var token in oldTokens)
            {
                token.UsedAt = now;
                token.UpdatedAt = now;
            }

            await _dbContext.SaveChangesAsync();

            var otp = await _verificationTokenService.CreateOtpAsync(
                user.Id,
                VerificationTokenPurpose.PasswordReset,
                PasswordResetOtpExpiryMinutes);

            try
            {
                await _emailService.SendResetPasswordEmailAsync(user.Email, otp);
            }
            catch
            {
                return ApiResponse<object>.Fail("Password reset email could not be sent.");
            }

            return ApiResponse<object>.Ok(null, PasswordResetSentMessage);
        }

        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var errors = ValidateResetPasswordRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<object>.Fail("Validation failed", errors);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user is null)
            {
                return ApiResponse<object>.Fail("Invalid reset request.");
            }

            var isValidOtp = await _verificationTokenService.VerifyOtpAsync(
                user.Id,
                VerificationTokenPurpose.PasswordReset,
                request.Otp);

            if (!isValidOtp)
            {
                return ApiResponse<object>.Fail("Invalid or expired reset code.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Password reset successfully.");
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            var errors = ValidateChangePasswordRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<object>.Fail("Validation failed", errors);
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                return ApiResponse<object>.Fail("User not found.");
            }

            if (user.Status != UserStatus.Active)
            {
                return ApiResponse<object>.Fail("User account is not active.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return ApiResponse<object>.Fail("Current password is incorrect.");
            }

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
            {
                return ApiResponse<object>.Fail("New password must be different from current password.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Password changed successfully.");
        }

        private async Task RevokeActiveRefreshTokensAsync(int userId, DateTimeOffset now)
        {
            var activeTokens = await _dbContext.RefreshTokens
                .Where(token => token.UserId == userId
                    && token.RevokedAt == null
                    && token.ExpiresAt > now)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.RevokedAt = now;
                token.UpdatedAt = now;
            }

            if (activeTokens.Count > 0)
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CleanupOldRefreshTokensAsync(DateTimeOffset now)
        {
            var cutoff = now.AddDays(-RefreshTokenCleanupRetentionDays);
            var oldTokens = await _dbContext.RefreshTokens
                .Where(token => token.ExpiresAt <= cutoff
                    || (token.RevokedAt != null && token.RevokedAt <= cutoff))
                .ToListAsync();

            if (oldTokens.Count == 0)
            {
                return;
            }

            _dbContext.RefreshTokens.RemoveRange(oldTokens);
            await _dbContext.SaveChangesAsync();
        }

        private static bool CanUseRotatedTokenInGracePeriod(RefreshToken token, DateTimeOffset now)
        {
            return token.RevokedAt is not null
                && !string.IsNullOrWhiteSpace(token.ReplacedByTokenHash)
                && token.RevokedAt.Value.AddSeconds(RefreshTokenReuseGraceSeconds) >= now;
        }

        private static string GetRefreshTokenGraceCacheKey(string tokenHash)
        {
            return $"refresh-token-grace:{tokenHash}";
        }

        private (string Value, RefreshToken Entity) CreateRefreshToken(int userId, DateTimeOffset? createdAt = null)
        {
            var value = GenerateSecureRefreshToken();
            var now = createdAt ?? DateTimeOffset.UtcNow;
            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = HashRefreshToken(value),
                ExpiresAt = now.AddDays(GetRefreshTokenExpiryDays()),
                CreatedAt = now,
                IsDeleted = false
            };

            return (value, entity);
        }

        private int GetRefreshTokenExpiryDays()
        {
            var jwtSection = _configuration.GetSection("Jwt");

            return int.TryParse(jwtSection["RefreshTokenExpiresInDays"], out var days) && days > 0
                ? days
                : DefaultRefreshTokenExpiryDays;
        }

        private static string GenerateSecureRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(RefreshTokenBytes));
        }

        private static string HashRefreshToken(string refreshToken)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToHexString(hash);
        }

        private static LoginUserDto BuildLoginUserDto(User user)
        {
            return new LoginUserDto
            {
                Id = user.Id,
                FullName = user.UserProfile?.FullName ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.Phone ?? string.Empty,
                Role = user.Role.ToString(),
                Status = user.Status.ToString(),
                IsEmailVerified = user.IsEmailVerified
            };
        }

        private static List<string> ValidateRefreshTokenRequest(string? refreshToken)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                errors.Add("RefreshToken is required");
            }

            return errors;
        }

        private static List<string> ValidateRequest(RegisterRequestDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                errors.Add("FullName is required");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("Email is required");
            }
            else if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                errors.Add("Invalid email format");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                errors.Add("PhoneNumber is required");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors.Add("Password is required");
            }
            else if (request.Password.Length < 6)
            {
                errors.Add("Password must be at least 6 characters long");
            }

            if (string.IsNullOrWhiteSpace(request.Role))
            {
                errors.Add("Role is required");
            }
            else if (!IsAllowedFrontendRole(request.Role))
            {
                errors.Add("Invalid role. Allowed values: Player, Owner");
            }
            else if (string.Equals(request.Role.Trim(), OwnerRole, StringComparison.Ordinal)
                && string.IsNullOrWhiteSpace(request.BusinessName))
            {
                errors.Add("BusinessName is required when role is Owner");
            }

            return errors;
        }

        private static List<string> ValidateForgotPasswordRequest(ForgotPasswordRequestDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("Email is required");
            }
            else if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                errors.Add("Invalid email format");
            }

            return errors;
        }

        private static List<string> ValidateResetPasswordRequest(ResetPasswordRequestDto request)
        {
            var errors = ValidateForgotPasswordRequest(new ForgotPasswordRequestDto
            {
                Email = request.Email
            });

            if (string.IsNullOrWhiteSpace(request.Otp))
            {
                errors.Add("Otp is required");
            }
            else if (request.Otp.Trim().Length != 6 || !request.Otp.Trim().All(char.IsDigit))
            {
                errors.Add("Otp must be 6 digits");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                errors.Add("NewPassword is required");
            }
            else if (request.NewPassword.Length < 6)
            {
                errors.Add("NewPassword must be at least 6 characters long");
            }

            return errors;
        }

        private static List<string> ValidateChangePasswordRequest(ChangePasswordRequestDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                errors.Add("CurrentPassword is required");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                errors.Add("NewPassword is required");
            }
            else if (request.NewPassword.Length < 6)
            {
                errors.Add("NewPassword must be at least 6 characters long");
            }

            if (!string.IsNullOrWhiteSpace(request.CurrentPassword)
                && !string.IsNullOrWhiteSpace(request.NewPassword)
                && request.CurrentPassword == request.NewPassword)
            {
                errors.Add("New password must be different from current password");
            }

            return errors;
        }

        private static List<string> ValidateLoginRequest(LoginRequestDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Identifier))
            {
                errors.Add("Identifier is required");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors.Add("Password is required");
            }

            return errors;
        }

        private static List<string> ValidateVerifyEmailRequest(VerifyEmailRequestDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("Email is required");
            }
            else if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                errors.Add("Invalid email format");
            }

            if (string.IsNullOrWhiteSpace(request.Otp))
            {
                errors.Add("Otp is required");
            }
            else if (request.Otp.Trim().Length != 6)
            {
                errors.Add("Otp must be 6 digits");
            }

            return errors;
        }

        private static List<string> ValidateResendVerifyEmailRequest(ResendVerifyEmailRequestDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("Email is required");
            }
            else if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                errors.Add("Invalid email format");
            }

            return errors;
        }

        private Task<VerificationToken?> GetLatestUnusedEmailVerificationTokenAsync(int userId)
        {
            return _dbContext.VerificationTokens
                .Where(t => t.UserId == userId
                    && t.Purpose == VerificationTokenPurpose.EmailVerification
                    && t.UsedAt == null)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        private static bool IsAllowedFrontendRole(string role)
        {
            return string.Equals(role.Trim(), PlayerRole, StringComparison.Ordinal)
                || string.Equals(role.Trim(), OwnerRole, StringComparison.Ordinal);
        }

        private static UserRole MapRole(string frontendRole)
        {
            return frontendRole == OwnerRole ? UserRole.CourtOwner : UserRole.Player;
        }

        private static string ToFrontendRole(UserRole userRole)
        {
            return userRole == UserRole.CourtOwner ? OwnerRole : PlayerRole;
        }
    }
}
