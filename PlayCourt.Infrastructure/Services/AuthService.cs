using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
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
        private const int ResendCooldownSeconds = 60;
        private const int MaxOtpFailedAttempts = 5;
        private readonly PlayCourtDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IVerificationTokenService _verificationTokenService;
        private readonly IEmailService _emailService;

        public AuthService(
            PlayCourtDbContext dbContext,
            IJwtTokenService jwtTokenService,
            IVerificationTokenService verificationTokenService,
            IEmailService emailService)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
            _verificationTokenService = verificationTokenService;
            _emailService = emailService;
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

            var token = _jwtTokenService.GenerateAccessToken(user, user.UserProfile);

            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
            {
                AccessToken = token.AccessToken,
                ExpiresAt = token.ExpiresAt,
                User = new LoginUserDto
                {
                    Id = user.Id,
                    FullName = user.UserProfile?.FullName ?? string.Empty,
                    Email = user.Email,
                    PhoneNumber = user.Phone ?? string.Empty,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString(),
                    IsEmailVerified = user.IsEmailVerified
                }
            }, "Login successfully");
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
