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

            if (ownerProfile.VerificationStatus != CourtOwnerVerificationStatus.Approved)
            {
                return ApiResponse<VenueResponseDto>.Fail(
                    "Court owner profile must be approved before creating a venue.");
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
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
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
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
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

        public async Task<ApiResponse<object>> DeleteVenueAsync(int userId, int venueId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null)
                return ApiResponse<object>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues
                .FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);

            if (venue is null)
                return ApiResponse<object>.Fail("Venue not found.");

            venue.IsDeleted = true;
            venue.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Venue deleted successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetAllVenuesAsync(VenueSearchRequestDto request)
        {
            var query = _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
                .Where(v => v.Status == VenueStatus.Approved);

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(v => v.Name.Contains(request.Keyword) || v.Address.Contains(request.Keyword));
            }

            // Simple pagination
            var venues = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(v => MapToResponse(v))
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(venues, "Venues retrieved successfully.");
        }

        public async Task<ApiResponse<VenueResponseDto>> GetPublicVenueByIdAsync(int venueId)
        {
            var venue = await _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
                .FirstOrDefaultAsync(v => v.Id == venueId && v.Status == VenueStatus.Approved);

            if (venue is null)
                return ApiResponse<VenueResponseDto>.Fail("Venue not found.");

            return ApiResponse<VenueResponseDto>.Ok(MapToResponse(venue), "Venue retrieved successfully.");
        }

        public async Task<ApiResponse<VenueImageDto>> AddImageAsync(int userId, int venueId, string imageUrl, bool isCover)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<VenueImageDto>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);
            if (venue is null) return ApiResponse<VenueImageDto>.Fail("Venue not found.");

            if (isCover)
            {
                var existingCover = await _dbContext.VenueImages.FirstOrDefaultAsync(i => i.VenueId == venueId && i.IsCover);
                if (existingCover != null) existingCover.IsCover = false;
            }

            var image = new VenueImage { VenueId = venueId, ImageUrl = imageUrl, IsCover = isCover };
            _dbContext.VenueImages.Add(image);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<VenueImageDto>.Ok(new VenueImageDto { Id = image.Id, VenueId = image.VenueId, ImageUrl = image.ImageUrl, IsCover = image.IsCover, CreatedAt = image.CreatedAt }, "Image added successfully.");
        }

        public async Task<ApiResponse<object>> DeleteImageAsync(int userId, int venueId, int imageId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<object>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);
            if (venue is null) return ApiResponse<object>.Fail("Venue not found.");

            var image = await _dbContext.VenueImages.FirstOrDefaultAsync(i => i.Id == imageId && i.VenueId == venueId);
            if (image is null) return ApiResponse<object>.Fail("Image not found.");

            _dbContext.VenueImages.Remove(image);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Image deleted successfully.");
        }

        public async Task<ApiResponse<VenueImageDto>> SetCoverImageAsync(int userId, int venueId, int imageId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<VenueImageDto>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);
            if (venue is null) return ApiResponse<VenueImageDto>.Fail("Venue not found.");

            var image = await _dbContext.VenueImages.FirstOrDefaultAsync(i => i.Id == imageId && i.VenueId == venueId);
            if (image is null) return ApiResponse<VenueImageDto>.Fail("Image not found.");

            var currentCover = await _dbContext.VenueImages.FirstOrDefaultAsync(i => i.VenueId == venueId && i.IsCover);
            if (currentCover != null) currentCover.IsCover = false;

            image.IsCover = true;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<VenueImageDto>.Ok(new VenueImageDto { Id = image.Id, VenueId = image.VenueId, ImageUrl = image.ImageUrl, IsCover = image.IsCover, CreatedAt = image.CreatedAt }, "Cover image set successfully.");
        }

        public async Task<ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>> AddVenueAmenityAsync(int userId, int venueId, int amenityId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);
            if (venue is null) return ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>.Fail("Venue not found.");

            var amenity = await _dbContext.Amenities.FirstOrDefaultAsync(a => a.Id == amenityId);
            if (amenity is null) return ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>.Fail("Amenity not found.");

            if (await _dbContext.VenueAmenities.AnyAsync(va => va.VenueId == venueId && va.AmenityId == amenityId))
                return ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>.Fail("Venue already has this amenity.");

            _dbContext.VenueAmenities.Add(new VenueAmenity { VenueId = venueId, AmenityId = amenityId });
            await _dbContext.SaveChangesAsync();

            return ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>.Ok(new PlayCourt.Application.DTOs.Amenities.AmenityDto { Id = amenity.Id, Name = amenity.Name }, "Amenity added successfully.");
        }

        public async Task<ApiResponse<object>> RemoveVenueAmenityAsync(int userId, int venueId, int amenityId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<object>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);
            if (venue is null) return ApiResponse<object>.Fail("Venue not found.");

            var venueAmenity = await _dbContext.VenueAmenities.FirstOrDefaultAsync(va => va.VenueId == venueId && va.AmenityId == amenityId);
            if (venueAmenity is null) return ApiResponse<object>.Fail("Venue does not have this amenity.");

            _dbContext.VenueAmenities.Remove(venueAmenity);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Amenity removed successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>> GetOpeningHoursAsync(int venueId)
        {
            var hours = await _dbContext.VenueOpeningHours
                .AsNoTracking()
                .Where(h => h.VenueId == venueId)
                .OrderBy(h => h.DayOfWeek)
                .Select(h => new VenueOpeningHourDto { Id = h.Id, VenueId = h.VenueId, DayOfWeek = h.DayOfWeek, OpenTime = h.OpenTime, CloseTime = h.CloseTime, IsClosed = h.IsClosed })
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>.Ok(hours, "Opening hours retrieved successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>> UpdateOpeningHoursAsync(int userId, int venueId, UpdateOpeningHoursRequestDto request)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);
            if (venue is null) return ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>.Fail("Venue not found.");

            var existingHours = await _dbContext.VenueOpeningHours.Where(h => h.VenueId == venueId).ToListAsync();
            _dbContext.VenueOpeningHours.RemoveRange(existingHours);

            var newHours = request.OpeningHours.Select(h => new VenueOpeningHour
            {
                VenueId = venueId,
                DayOfWeek = h.DayOfWeek,
                OpenTime = h.OpenTime,
                CloseTime = h.CloseTime,
                IsClosed = h.IsClosed
            }).ToList();

            _dbContext.VenueOpeningHours.AddRange(newHours);
            await _dbContext.SaveChangesAsync();

            return await GetOpeningHoursAsync(venueId);
        }

        public async Task<ApiResponse<VenueStatsResponseDto>> GetOwnerStatsAsync(int userId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<VenueStatsResponseDto>.Fail("Court owner profile not found.");

            var totalVenues = await _dbContext.Venues.CountAsync(v => v.CourtOwnerProfileId == ownerProfile.Id);
            var totalCourts = await _dbContext.Courts.CountAsync(c => c.Venue.CourtOwnerProfileId == ownerProfile.Id);

            // Assuming bookings table isn't created yet or we skip it for now, just mock TodayBookings
            var stats = new VenueStatsResponseDto
            {
                TotalVenues = totalVenues,
                TotalCourts = totalCourts,
                TodayBookings = 0 // Placeholder
            };

            return ApiResponse<VenueStatsResponseDto>.Ok(stats, "Stats retrieved successfully.");
        }

        public async Task<ApiResponse<VenueResponseDto>> GetMyVenueByIdAsync(int userId, int venueId)
        {
            return await GetVenueByIdAsync(userId, venueId);
        }

        public async Task<ApiResponse<FavoriteVenueResponseDto>> AddFavoriteAsync(int userId, int venueId)
        {
            var userProfile = await _dbContext.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (userProfile is null)
                return ApiResponse<FavoriteVenueResponseDto>.Fail("User profile not found.");

            var venue = await _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
                .FirstOrDefaultAsync(v => v.Id == venueId && v.Status == VenueStatus.Approved);

            if (venue is null)
                return ApiResponse<FavoriteVenueResponseDto>.Fail("Venue not found or not approved.");

            var alreadyFavorited = await _dbContext.UserFavoriteVenues
                .AnyAsync(f => f.UserProfileId == userProfile.Id && f.VenueId == venueId);

            if (alreadyFavorited)
                return ApiResponse<FavoriteVenueResponseDto>.Fail("Venue is already in your favorites.");

            var favorite = new UserFavoriteVenue
            {
                UserProfileId = userProfile.Id,
                VenueId = venueId,
                CreatedAt = DateTimeOffset.Now,
            };

            _dbContext.UserFavoriteVenues.Add(favorite);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<FavoriteVenueResponseDto>.Ok(
                new FavoriteVenueResponseDto
                {
                    UserProfileId = favorite.UserProfileId,
                    VenueId = favorite.VenueId,
                    CreatedAt = favorite.CreatedAt,
                    Venue = MapToResponse(venue),
                },
                "Venue added to favorites.");
        }

        public async Task<ApiResponse<object>> RemoveFavoriteAsync(int userId, int venueId)
        {
            var userProfile = await _dbContext.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (userProfile is null)
                return ApiResponse<object>.Fail("User profile not found.");

            var favorite = await _dbContext.UserFavoriteVenues
                .FirstOrDefaultAsync(f => f.UserProfileId == userProfile.Id && f.VenueId == venueId);

            if (favorite is null)
                return ApiResponse<object>.Fail("Venue is not in your favorites.");

            _dbContext.UserFavoriteVenues.Remove(favorite);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Venue removed from favorites.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<FavoriteVenueResponseDto>>> GetMyFavoritesAsync(int userId)
        {
            var userProfile = await _dbContext.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (userProfile is null)
                return ApiResponse<IReadOnlyCollection<FavoriteVenueResponseDto>>.Fail("User profile not found.");

            var favorites = await _dbContext.UserFavoriteVenues
                .AsNoTracking()
                .Include(f => f.Venue)
                    .ThenInclude(v => v.Images)
                .Include(f => f.Venue)
                    .ThenInclude(v => v.VenueAmenities)
                        .ThenInclude(va => va.Amenity)
                .Include(f => f.Venue)
                    .ThenInclude(v => v.OpeningHours)
                .Where(f => f.UserProfileId == userProfile.Id)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FavoriteVenueResponseDto
                {
                    UserProfileId = f.UserProfileId,
                    VenueId = f.VenueId,
                    CreatedAt = f.CreatedAt,
                    Venue = MapToResponse(f.Venue),
                })
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<FavoriteVenueResponseDto>>.Ok(
                favorites,
                "Favorite venues retrieved successfully.");
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
                UpdatedAt = venue.UpdatedAt,
                Images = venue.Images?.Select(i => new VenueImageDto
                {
                    Id = i.Id,
                    VenueId = i.VenueId,
                    ImageUrl = i.ImageUrl,
                    IsCover = i.IsCover,
                    CreatedAt = i.CreatedAt
                }).ToList() ?? new List<VenueImageDto>(),
                Amenities = venue.VenueAmenities?.Select(va => new PlayCourt.Application.DTOs.Amenities.AmenityDto
                {
                    Id = va.Amenity.Id,
                    Name = va.Amenity.Name
                }).ToList() ?? new List<PlayCourt.Application.DTOs.Amenities.AmenityDto>(),
                OpeningHours = venue.OpeningHours?.Select(oh => new VenueOpeningHourDto
                {
                    Id = oh.Id,
                    VenueId = oh.VenueId,
                    DayOfWeek = oh.DayOfWeek,
                    OpenTime = oh.OpenTime,
                    CloseTime = oh.CloseTime,
                    IsClosed = oh.IsClosed
                }).ToList() ?? new List<VenueOpeningHourDto>()
            };
        }
    }
}
