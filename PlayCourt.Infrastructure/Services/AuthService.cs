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
        private readonly PlayCourtDbContext _dbContext;

        public AuthService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
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
            }, "Register successfully");
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
