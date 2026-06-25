using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Helpers;

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
                VenueMapper.MapToResponse(venue),
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
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(
                venues.Select(VenueMapper.MapToResponse).ToList(),
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
                VenueMapper.MapToResponse(venue),
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
                VenueMapper.MapToResponse(venue),
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

        public async Task<PagedResponse<IReadOnlyCollection<VenueResponseDto>>> GetAllVenuesAsync(VenueSearchRequestDto request)
        {
            var query = _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
                .Where(v => v.Status == VenueStatus.Approved && !v.IsDeleted);

            // Filter theo từ khoá tên hoặc địa chỉ
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim();
                query = query.Where(v => v.Name.Contains(kw) || v.Address.Contains(kw));
            }

            // Filter theo bộ môn thể thao (qua Courts.SportId)
            if (request.SportId.HasValue)
            {
                query = query.Where(v => v.Courts.Any(c => c.SportId == request.SportId.Value && !c.IsDeleted));
            }

            // Filter theo sân đang mở cửa tại thời điểm hiện tại
            if (request.IsOpenNow == true)
            {
                var now = TimeSpan.FromTicks(DateTimeOffset.Now.TimeOfDay.Ticks);
                var today = (int)DateTime.Today.DayOfWeek == 0 ? 7 : (int)DateTime.Today.DayOfWeek; // 1=Mon..7=Sun
                query = query.Where(v =>
                    v.OpeningHours.Any(oh =>
                        oh.DayOfWeek == today &&
                        !oh.IsClosed &&
                        oh.OpenTime.HasValue &&
                        oh.CloseTime.HasValue &&
                        oh.OpenTime.Value <= now &&
                        oh.CloseTime.Value > now));
            }

            // Đếm tổng trước khi phân trang
            var totalCount = await query.CountAsync();

            var venues = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var result = venues.Select(VenueMapper.MapToResponse).ToList();

            return PagedResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(
                result,
                totalCount,
                request.PageIndex,
                request.PageSize,
                "Venues retrieved successfully.");
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

            return ApiResponse<VenueResponseDto>.Ok(VenueMapper.MapToResponse(venue), "Venue retrieved successfully.");
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
            // API public: chỉ trả về giờ mở cửa của venue đã được Approved
            var venueExists = await _dbContext.Venues
                .AnyAsync(v => v.Id == venueId && v.Status == VenueStatus.Approved && !v.IsDeleted);

            if (!venueExists)
                return ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>.Fail("Venue not found.");

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

            // Validate từng entry trước khi chạm vào DB
            var validationErrors = new List<string>();
            var days = new HashSet<int>();

            for (var i = 0; i < request.OpeningHours.Count; i++)
            {
                var h = request.OpeningHours[i];

                if (h.DayOfWeek < 1 || h.DayOfWeek > 7)
                    validationErrors.Add($"Entry [{i}]: DayOfWeek phải nằm trong khoảng 1 (Thứ Hai) đến 7 (Chủ Nhật).");

                if (!days.Add(h.DayOfWeek))
                    validationErrors.Add($"Entry [{i}]: DayOfWeek '{h.DayOfWeek}' bị trùng lặp.");

                if (!h.IsClosed)
                {
                    if (!h.OpenTime.HasValue || !h.CloseTime.HasValue)
                        validationErrors.Add($"Entry [{i}]: OpenTime và CloseTime là bắt buộc khi IsClosed = false.");
                    else if (h.OpenTime >= h.CloseTime)
                        validationErrors.Add($"Entry [{i}]: OpenTime phải nhỏ hơn CloseTime.");
                }
            }

            if (validationErrors.Count > 0)
                return ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>.Fail("Dữ liệu giờ mở cửa không hợp lệ.", validationErrors);

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

            // Lấy lại trực tiếp từ danh sách đã lưu (không qua public endpoint để tránh check Approved)
            var savedHours = newHours.Select(h => new VenueOpeningHourDto
            {
                Id = h.Id,
                VenueId = h.VenueId,
                DayOfWeek = h.DayOfWeek,
                OpenTime = h.OpenTime,
                CloseTime = h.CloseTime,
                IsClosed = h.IsClosed
            }).ToList();

            return ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>.Ok(savedHours, "Opening hours updated successfully.");
        }

        public async Task<ApiResponse<VenueStatsResponseDto>> GetOwnerStatsAsync(int userId)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<VenueStatsResponseDto>.Fail("Court owner profile not found.");

            var totalVenues = await _dbContext.Venues
                .CountAsync(v => v.CourtOwnerProfileId == ownerProfile.Id && !v.IsDeleted);

            var totalCourts = await _dbContext.Courts
                .CountAsync(c => c.Venue.CourtOwnerProfileId == ownerProfile.Id && !c.IsDeleted);

            // Đếm số lượng booking hôm nay (tất cả trạng thái trừ đã huỷ)
            var todayStart = DateTimeOffset.Now.Date;
            var todayEnd = todayStart.AddDays(1);
            var todayBookings = await _dbContext.Bookings
                .CountAsync(b =>
                    b.Court.Venue.CourtOwnerProfileId == ownerProfile.Id &&
                    b.StartAt >= todayStart &&
                    b.StartAt < todayEnd &&
                    b.Status != BookingStatus.CancelledByUser &&
                    b.Status != BookingStatus.CancelledByOwner &&
                    !b.IsDeleted);

            var stats = new VenueStatsResponseDto
            {
                TotalVenues = totalVenues,
                TotalCourts = totalCourts,
                TodayBookings = todayBookings
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
                    Venue = VenueMapper.MapToResponse(venue),
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

            // Chỉ trả về favorite của các venue đang ở trạng thái Approved (không hiển thị venue bị Suspend/Reject)
            var favorites = await _dbContext.UserFavoriteVenues
                .AsNoTracking()
                .Include(f => f.Venue)
                    .ThenInclude(v => v.Images)
                .Include(f => f.Venue)
                    .ThenInclude(v => v.VenueAmenities)
                        .ThenInclude(va => va.Amenity)
                .Include(f => f.Venue)
                    .ThenInclude(v => v.OpeningHours)
                .Where(f =>
                    f.UserProfileId == userProfile.Id &&
                    f.Venue.Status == VenueStatus.Approved &&
                    !f.Venue.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            var result = favorites.Select(f => new FavoriteVenueResponseDto
            {
                UserProfileId = f.UserProfileId,
                VenueId = f.VenueId,
                CreatedAt = f.CreatedAt,
                Venue = VenueMapper.MapToResponse(f.Venue),
            }).ToList();

            return ApiResponse<IReadOnlyCollection<FavoriteVenueResponseDto>>.Ok(
                result,
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

    }
}
