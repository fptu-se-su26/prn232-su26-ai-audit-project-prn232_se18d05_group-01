using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtOwners;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class CourtOwnerService : ICourtOwnerService
    {
        private readonly PlayCourtDbContext _dbContext;

        public CourtOwnerService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<List<CourtOwnerListItemDto>>> GetAllAsync(
            CourtOwnerVerificationStatus? verificationStatus = null)
        {
            if (verificationStatus.HasValue && !Enum.IsDefined(verificationStatus.Value))
            {
                return ApiResponse<List<CourtOwnerListItemDto>>.Fail("Verification status is invalid.");
            }

            var query = _dbContext.CourtOwnerProfiles.AsNoTracking().AsQueryable();

            if (verificationStatus.HasValue)
            {
                query = query.Where(profile => profile.VerificationStatus == verificationStatus.Value);
            }

            var profiles = await query
                .Include(profile => profile.UserProfile)
                .ThenInclude(profile => profile.User)
                .OrderByDescending(profile => profile.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<CourtOwnerListItemDto>>.Ok(
                profiles.Select(MapToListItem).ToList(),
                "Court owner profiles retrieved successfully.");
        }

        public async Task<ApiResponse<CourtOwnerDetailDto>> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            var profile = await FindProfileAsync(id, asNoTracking: true);
            if (profile is null)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            return ApiResponse<CourtOwnerDetailDto>.Ok(
                MapToDetail(profile),
                "Court owner profile retrieved successfully.");
        }

        public async Task<ApiResponse<CourtOwnerDetailDto>> UpdateVerificationStatusAsync(
            int id,
            UpdateCourtOwnerVerificationStatusRequestDto request)
        {
            if (id <= 0)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            if (!Enum.IsDefined(typeof(CourtOwnerVerificationStatus), request.VerificationStatus) ||
                request.VerificationStatus == (int)CourtOwnerVerificationStatus.Pending)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail(
                    "Verification status must be Approved or Rejected.");
            }

            var profile = await FindProfileAsync(id, asNoTracking: false);
            if (profile is null)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            var verificationStatus = (CourtOwnerVerificationStatus)request.VerificationStatus;
            profile.VerificationStatus = verificationStatus;
            profile.RejectionReason = verificationStatus == CourtOwnerVerificationStatus.Rejected
                ? NormalizeOptional(request.RejectionReason)
                : null;
            profile.UpdatedAt = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<CourtOwnerDetailDto>.Ok(
                MapToDetail(profile),
                "Court owner verification status updated successfully.");
        }

        private async Task<CourtOwnerProfile?> FindProfileAsync(int id, bool asNoTracking)
        {
            var query = _dbContext.CourtOwnerProfiles
                .Include(profile => profile.UserProfile)
                .ThenInclude(profile => profile.User)
                .Where(profile => profile.Id == id);

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        private static CourtOwnerListItemDto MapToListItem(CourtOwnerProfile profile)
        {
            return new CourtOwnerListItemDto
            {
                Id = profile.Id,
                UserId = profile.UserProfile.UserId,
                FullName = profile.UserProfile.FullName,
                Email = profile.UserProfile.User.Email,
                Phone = profile.UserProfile.User.Phone,
                BusinessName = profile.BusinessName,
                VerificationStatus = profile.VerificationStatus.ToString(),
                CreatedAt = profile.CreatedAt
            };
        }

        private static CourtOwnerDetailDto MapToDetail(CourtOwnerProfile profile)
        {
            return new CourtOwnerDetailDto
            {
                Id = profile.Id,
                UserProfileId = profile.UserProfileId,
                UserId = profile.UserProfile.UserId,
                FullName = profile.UserProfile.FullName,
                Email = profile.UserProfile.User.Email,
                Phone = profile.UserProfile.User.Phone,
                AvatarUrl = profile.UserProfile.AvatarUrl,
                DateOfBirth = profile.UserProfile.DateOfBirth,
                Gender = profile.UserProfile.Gender?.ToString(),
                Address = profile.UserProfile.Address,
                City = profile.UserProfile.City,
                Country = profile.UserProfile.Country,
                BusinessName = profile.BusinessName,
                BusinessLicenseNo = profile.BusinessLicenseNo,
                TaxCode = profile.TaxCode,
                BusinessAddress = profile.BusinessAddress,
                VerificationStatus = profile.VerificationStatus.ToString(),
                RejectionReason = profile.RejectionReason,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
