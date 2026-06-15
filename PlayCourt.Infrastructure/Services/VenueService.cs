using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class VenueService : IVenueService
    {
        private readonly PlayCourtDbContext _dbContext;

        public VenueService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<VenueResponseDto>> CreateVenueAsync(
            int userId,
            CreateVenueRequestDto request)
        {
            var errors = ValidateVenueRequest(
                request.Name,
                request.Address,
                request.Latitude,
                request.Longitude,
                request.OpenTime,
                request.CloseTime);

            if (errors.Count > 0)
            {
                return ApiResponse<VenueResponseDto>.Fail("Create venue failed.", errors);
            }

            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null)
            {
                return ApiResponse<VenueResponseDto>.Fail("Court owner profile not found.");
            }

            var venue = new Venue
            {
                CourtOwnerProfileId = ownerProfile.Id,
                Name = request.Name!.Trim(),
                Description = NormalizeOptional(request.Description),
                Address = request.Address!.Trim(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Phone = NormalizeOptional(request.Phone),
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                Status = VenueStatus.Pending,
                CreatedAt = DateTimeOffset.Now,
                IsDeleted = false
            };

            _dbContext.Venues.Add(venue);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<VenueResponseDto>.Ok(
                MapToResponse(venue),
                "Venue created successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetMyVenuesAsync(int userId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null)
            {
                return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Fail("Court owner profile not found.");
            }

            var venues = await _dbContext.Venues
                .AsNoTracking()
                .Where(venue => venue.CourtOwnerProfileId == ownerProfile.Id)
                .OrderByDescending(venue => venue.CreatedAt)
                .Select(venue => MapToResponse(venue))
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(
                venues,
                "Venues retrieved successfully.");
        }

        public async Task<ApiResponse<VenueResponseDto>> GetVenueByIdAsync(
            int userId,
            int venueId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null)
            {
                return ApiResponse<VenueResponseDto>.Fail("Court owner profile not found.");
            }

            var venue = await _dbContext.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(venue =>
                    venue.Id == venueId &&
                    venue.CourtOwnerProfileId == ownerProfile.Id);

            if (venue is null)
            {
                return ApiResponse<VenueResponseDto>.Fail("Venue not found.");
            }

            return ApiResponse<VenueResponseDto>.Ok(
                MapToResponse(venue),
                "Venue retrieved successfully.");
        }

        public async Task<ApiResponse<VenueResponseDto>> UpdateVenueAsync(
            int userId,
            int venueId,
            UpdateVenueRequestDto request)
        {
            var errors = ValidateVenueRequest(
                request.Name,
                request.Address,
                request.Latitude,
                request.Longitude,
                request.OpenTime,
                request.CloseTime);

            if (errors.Count > 0)
            {
                return ApiResponse<VenueResponseDto>.Fail("Update venue failed.", errors);
            }

            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null)
            {
                return ApiResponse<VenueResponseDto>.Fail("Court owner profile not found.");
            }

            var venue = await _dbContext.Venues
                .FirstOrDefaultAsync(venue =>
                    venue.Id == venueId &&
                    venue.CourtOwnerProfileId == ownerProfile.Id);

            if (venue is null)
            {
                return ApiResponse<VenueResponseDto>.Fail("Venue not found.");
            }

            venue.Name = request.Name!.Trim();
            venue.Description = NormalizeOptional(request.Description);
            venue.Address = request.Address!.Trim();
            venue.Latitude = request.Latitude;
            venue.Longitude = request.Longitude;
            venue.Phone = NormalizeOptional(request.Phone);
            venue.OpenTime = request.OpenTime;
            venue.CloseTime = request.CloseTime;
            venue.Status = VenueStatus.Pending;
            venue.UpdatedAt = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<VenueResponseDto>.Ok(
                MapToResponse(venue),
                "Venue updated successfully.");
        }

        private async Task<CourtOwnerProfile?> FindCourtOwnerProfileAsync(int userId)
        {
            return await _dbContext.CourtOwnerProfiles
                .Include(profile => profile.UserProfile)
                .ThenInclude(profile => profile.User)
                .FirstOrDefaultAsync(profile =>
                    profile.UserProfile.UserId == userId &&
                    profile.UserProfile.User.Role == UserRole.CourtOwner);
        }

        private static List<string> ValidateVenueRequest(
            string? name,
            string? address,
            decimal? latitude,
            decimal? longitude,
            TimeSpan? openTime,
            TimeSpan? closeTime)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                errors.Add("Address is required.");
            }

            if (latitude is < -90 or > 90)
            {
                errors.Add("Latitude must be between -90 and 90.");
            }

            if (longitude is < -180 or > 180)
            {
                errors.Add("Longitude must be between -180 and 180.");
            }

            if (openTime.HasValue && closeTime.HasValue && openTime >= closeTime)
            {
                errors.Add("OpenTime must be before CloseTime.");
            }

            return errors;
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static VenueResponseDto MapToResponse(Venue venue)
        {
            return new VenueResponseDto
            {
                Id = venue.Id,
                CourtOwnerProfileId = venue.CourtOwnerProfileId,
                Name = venue.Name,
                Description = venue.Description,
                Address = venue.Address,
                Latitude = venue.Latitude,
                Longitude = venue.Longitude,
                Phone = venue.Phone,
                OpenTime = venue.OpenTime,
                CloseTime = venue.CloseTime,
                Status = venue.Status.ToString(),
                CreatedAt = venue.CreatedAt,
                UpdatedAt = venue.UpdatedAt
            };
        }
    }
}
