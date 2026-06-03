using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Users;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class UserService : IUserService
    {
        private readonly PlayCourtDbContext _dbContext;

        public UserService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<UserProfileResponseDto>> GetCurrentUserProfileAsync(int userId)
        {
            var profile = await FindProfileAsync(userId);

            if (profile is null)
            {
                return ApiResponse<UserProfileResponseDto>.Fail("User profile not found.");
            }

            return ApiResponse<UserProfileResponseDto>.Ok(
                MapToResponse(profile),
                "User profile retrieved successfully.");
        }

        public async Task<ApiResponse<UserProfileResponseDto>> UpdateCurrentUserProfileAsync(
            int userId,
            UpdateUserProfileRequestDto request)
        {
            var errors = ValidateRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<UserProfileResponseDto>.Fail("Update user profile failed.", errors);
            }

            var profile = await FindProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<UserProfileResponseDto>.Fail("User profile not found.");
            }

            profile.FullName = request.FullName!.Trim();
            profile.AvatarUrl = NormalizeOptional(request.AvatarUrl);
            profile.DateOfBirth = request.DateOfBirth;
            profile.Gender = request.Gender.HasValue ? (Gender)request.Gender.Value : null;
            profile.Address = NormalizeOptional(request.Address);
            profile.City = NormalizeOptional(request.City);
            profile.Country = NormalizeOptional(request.Country);
            profile.UpdatedAt = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<UserProfileResponseDto>.Ok(
                MapToResponse(profile),
                "User profile updated successfully.");
        }

        private async Task<UserProfile?> FindProfileAsync(int userId)
        {
            return await _dbContext.UserProfiles
                .Include(profile => profile.User)
                .Include(profile => profile.CourtOwnerProfile)
                .FirstOrDefaultAsync(profile => profile.UserId == userId);
        }

        private static List<string> ValidateRequest(UpdateUserProfileRequestDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                errors.Add("FullName is required.");
            }

            if (request.Gender.HasValue && !Enum.IsDefined(typeof(Gender), (short)request.Gender.Value))
            {
                errors.Add("Gender is invalid.");
            }

            return errors;
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static UserProfileResponseDto MapToResponse(UserProfile profile)
        {
            return new UserProfileResponseDto
            {
                UserId = profile.UserId,
                ProfileId = profile.Id,
                Email = profile.User.Email,
                Phone = profile.User.Phone,
                Role = profile.User.Role.ToString(),
                Status = profile.User.Status.ToString(),
                IsEmailVerified = profile.User.IsEmailVerified,
                FullName = profile.FullName,
                AvatarUrl = profile.AvatarUrl,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                Address = profile.Address,
                City = profile.City,
                Country = profile.Country,
                CourtOwnerProfile = profile.CourtOwnerProfile is null
                    ? null
                    : new CourtOwnerProfileSummaryDto
                    {
                        Id = profile.CourtOwnerProfile.Id,
                        BusinessName = profile.CourtOwnerProfile.BusinessName,
                        BusinessLicenseNo = profile.CourtOwnerProfile.BusinessLicenseNo,
                        TaxCode = profile.CourtOwnerProfile.TaxCode,
                        BusinessAddress = profile.CourtOwnerProfile.BusinessAddress,
                        VerificationStatus = profile.CourtOwnerProfile.VerificationStatus.ToString()
                    }
            };
        }
    }
}
