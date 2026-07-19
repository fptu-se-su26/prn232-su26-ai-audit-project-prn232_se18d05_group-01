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

        public async Task<ApiResponse<List<PlayerSportResponseDto>>> GetCurrentUserSportsAsync(int userId)
        {
            var profile = await _dbContext.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.UserId == userId);

            if (profile is null)
            {
                return ApiResponse<List<PlayerSportResponseDto>>.Fail("User profile not found.");
            }

            var playerSports = await _dbContext.PlayerSports
                .AsNoTracking()
                .Include(item => item.Sport)
                .Where(item => item.UserProfileId == profile.Id)
                .OrderBy(item => item.Sport.Name)
                .ToListAsync();

            return ApiResponse<List<PlayerSportResponseDto>>.Ok(
                playerSports.Select(MapPlayerSportToResponse).ToList(),
                "User sports retrieved successfully.");
        }

        public async Task<ApiResponse<PlayerSportResponseDto>> AddCurrentUserSportAsync(
            int userId,
            AddPlayerSportRequestDto request)
        {
            if (request.SportId <= 0)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Sport not found.");
            }

            if (!IsValidSkillLevel(request.SkillLevel))
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Skill level is invalid.");
            }

            var profile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(item => item.UserId == userId);

            if (profile is null)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("User profile not found.");
            }

            var sport = await _dbContext.Sports
                .FirstOrDefaultAsync(item => item.Id == request.SportId);

            if (sport is null)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Sport not found.");
            }

            if (!sport.IsActive)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Sport is inactive.");
            }

            if (await _dbContext.PlayerSports.AnyAsync(
                    item => item.UserProfileId == profile.Id && item.SportId == sport.Id))
            {
                return ApiResponse<PlayerSportResponseDto>.Fail(
                    "Sport already exists in user profile.");
            }

            var playerSport = new PlayerSport
            {
                UserProfileId = profile.Id,
                SportId = sport.Id,
                SkillLevel = (SkillLevel)request.SkillLevel,
                Sport = sport
            };

            _dbContext.PlayerSports.Add(playerSport);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<PlayerSportResponseDto>.Ok(
                MapPlayerSportToResponse(playerSport),
                "Sport added to user profile successfully.");
        }

        public async Task<ApiResponse<PlayerSportResponseDto>> UpdateCurrentUserSportAsync(
            int userId,
            int sportId,
            UpdatePlayerSportRequestDto request)
        {
            if (!IsValidSkillLevel(request.SkillLevel))
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Skill level is invalid.");
            }

            var profile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(item => item.UserId == userId);

            if (profile is null)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("User profile not found.");
            }

            if (sportId <= 0)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Sport is not in user profile.");
            }

            var playerSport = await _dbContext.PlayerSports
                .Include(item => item.Sport)
                .FirstOrDefaultAsync(
                    item => item.UserProfileId == profile.Id && item.SportId == sportId);

            if (playerSport is null)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Sport is not in user profile.");
            }

            playerSport.SkillLevel = (SkillLevel)request.SkillLevel;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<PlayerSportResponseDto>.Ok(
                MapPlayerSportToResponse(playerSport),
                "User sport skill level updated successfully.");
        }

        public async Task<ApiResponse<PlayerSportResponseDto>> RemoveCurrentUserSportAsync(
            int userId,
            int sportId)
        {
            var profile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(item => item.UserId == userId);

            if (profile is null)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("User profile not found.");
            }

            if (sportId <= 0)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Sport is not in user profile.");
            }

            var playerSport = await _dbContext.PlayerSports
                .Include(item => item.Sport)
                .FirstOrDefaultAsync(
                    item => item.UserProfileId == profile.Id && item.SportId == sportId);

            if (playerSport is null)
            {
                return ApiResponse<PlayerSportResponseDto>.Fail("Sport is not in user profile.");
            }

            var response = MapPlayerSportToResponse(playerSport);
            _dbContext.PlayerSports.Remove(playerSport);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<PlayerSportResponseDto>.Ok(
                response,
                "Sport removed from user profile successfully.");
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

        private static bool IsValidSkillLevel(int skillLevel)
        {
            return Enum.IsDefined(typeof(SkillLevel), (short)skillLevel);
        }

        private static PlayerSportResponseDto MapPlayerSportToResponse(PlayerSport playerSport)
        {
            return new PlayerSportResponseDto
            {
                Id = playerSport.Id,
                SportId = playerSport.SportId,
                SportCode = playerSport.Sport.Code,
                SportName = playerSport.Sport.Name,
                SkillLevel = playerSport.SkillLevel.ToString(),
                CreatedAt = playerSport.CreatedAt
            };
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
                        BusinessLicenseDocumentUrl = profile.CourtOwnerProfile.BusinessLicenseDocumentUrl,
                        VerificationStatus = profile.CourtOwnerProfile.VerificationStatus.ToString(),
                        RejectionReason = profile.CourtOwnerProfile.RejectionReason,
                        SubmittedAt = profile.CourtOwnerProfile.SubmittedAt
                    }
            };
        }
    }
}
