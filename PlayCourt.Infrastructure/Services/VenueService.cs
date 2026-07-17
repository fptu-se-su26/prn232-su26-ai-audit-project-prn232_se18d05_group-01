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

        private static readonly Dictionary<VenueStatus, HashSet<VenueStatus>> _allowedTransitions = new()
        {
            [VenueStatus.Pending] = new() { VenueStatus.Approved, VenueStatus.Rejected },
            [VenueStatus.Approved] = new() { VenueStatus.Suspended },
            [VenueStatus.Rejected] = new(),
            [VenueStatus.Suspended] = new() { VenueStatus.Approved },
        };

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
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(
                venues.Select(MapToResponse).ToList(),
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

            venue.Description = NormalizeOptional(request.Description);
            venue.Phone = NormalizeOptional(request.Phone);
            venue.OpenTime = request.OpenTime;
            venue.CloseTime = request.CloseTime;
            venue.UpdatedAt = DateTimeOffset.Now;

            var name = request.Name!.Trim();
            var address = request.Address!.Trim();
            var requiresApproval = venue.Status == VenueStatus.Approved &&
                (venue.Name != name || venue.Address != address ||
                 venue.Latitude != request.Latitude || venue.Longitude != request.Longitude);

            if (requiresApproval)
            {
                var changeRequest = await _dbContext.VenueChangeRequests
                    .FirstOrDefaultAsync(item =>
                        item.VenueId == venueId &&
                        item.Status == VenueChangeRequestStatus.Pending);

                if (changeRequest is null)
                {
                    _dbContext.VenueChangeRequests.Add(new VenueChangeRequest
                    {
                        VenueId = venueId,
                        Name = name,
                        Address = address,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        CreatedAt = DateTimeOffset.Now
                    });
                }
                else
                {
                    changeRequest.Name = name;
                    changeRequest.Address = address;
                    changeRequest.Latitude = request.Latitude;
                    changeRequest.Longitude = request.Longitude;
                    changeRequest.UpdatedAt = DateTimeOffset.Now;
                }
            }
            else
            {
                venue.Name = name;
                venue.Address = address;
                venue.Latitude = request.Latitude;
                venue.Longitude = request.Longitude;
            }

            await _dbContext.SaveChangesAsync();

            return ApiResponse<VenueResponseDto>.Ok(
                MapToResponse(venue),
                requiresApproval
                    ? "Operational information updated. The name or address change is pending approval."
                    : "Venue updated successfully.");
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

            // Chặn xóa nếu còn Booking tương lai Pending/Confirmed ở bất kỳ Court nào của Venue.
            var now = DateTimeOffset.Now;
            var hasFutureBookings = await _dbContext.Bookings
                .AnyAsync(b =>
                    b.Court.VenueId == venueId &&
                    !b.IsDeleted &&
                    b.StartAt > now &&
                    (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed));

            if (hasFutureBookings)
            {
                return ApiResponse<object>.Fail("Cannot delete venue because it still has pending or confirmed future bookings.");
            }

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

            var result = venues.Select(MapToResponse).ToList();

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

            return ApiResponse<VenueResponseDto>.Ok(MapToResponse(venue), "Venue retrieved successfully.");
        }

        public async Task<ApiResponse<VenueAvailabilityResponseDto>> GetAvailabilityAsync(int venueId, DateOnly date)
        {
            var venue = await _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.OpeningHours)
                .FirstOrDefaultAsync(v => v.Id == venueId && v.Status == VenueStatus.Approved && !v.IsDeleted);
            if (venue is null)
                return ApiResponse<VenueAvailabilityResponseDto>.Fail("Venue not found or not approved.");

            var dayOfWeek = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
            var openingHour = venue.OpeningHours.SingleOrDefault(h => h.DayOfWeek == dayOfWeek);
            var dayStart = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.Zero);
            var dayEnd = dayStart.AddDays(1);
            var courts = await _dbContext.Courts
                .AsNoTracking()
                .Include(c => c.Sport)
                .Where(c => c.VenueId == venueId && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Id)
                .ToListAsync();
            var courtIds = courts.Select(c => c.Id).ToList();
            var bookings = await _dbContext.Bookings
                .AsNoTracking()
                .Where(b => courtIds.Contains(b.CourtId) &&
                    (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                    b.StartAt < dayEnd && b.EndAt > dayStart)
                .ToListAsync();
            var matches = await _dbContext.Matches
                .AsNoTracking()
                .Where(m => courtIds.Contains(m.CourtId!.Value) &&
                    (m.Status == MatchStatus.Open || m.Status == MatchStatus.Full) &&
                    m.StartAt < dayEnd && m.EndAt > dayStart)
                .ToListAsync();
            var schedules = await _dbContext.CourtSchedules
                .AsNoTracking()
                .Where(s => courtIds.Contains(s.CourtId) && s.StartAt < dayEnd && s.EndAt > dayStart)
                .ToListAsync();
            var pricingRules = await _dbContext.PricingRules
                .AsNoTracking()
                .Where(r => courtIds.Contains(r.CourtId) && r.DayOfWeek == dayOfWeek &&
                    r.EffectiveFrom <= date.ToDateTime(TimeOnly.MinValue) &&
                    (r.EffectiveTo == null || r.EffectiveTo >= date.ToDateTime(TimeOnly.MinValue)))
                .ToListAsync();

            var result = new VenueAvailabilityResponseDto
            {
                Date = date,
                Venue = new VenueAvailabilityVenueDto
                {
                    Id = venue.Id,
                    Name = venue.Name,
                    Address = venue.Address,
                    OpenTime = openingHour?.OpenTime,
                    CloseTime = openingHour?.CloseTime,
                    IsClosed = openingHour?.IsClosed ?? false
                },
                Courts = courts.Select(court => BuildAvailabilityCourt(
                    court, dayStart, openingHour, bookings, matches, schedules, pricingRules)).ToList()
            };

            return ApiResponse<VenueAvailabilityResponseDto>.Ok(result, "Venue availability retrieved successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetAllVenuesForAdminAsync(
            VenueStatus? status = null)
        {
            if (status.HasValue && !Enum.IsDefined(status.Value))
            {
                return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Fail("Venue status is invalid.");
            }

            var query = _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }

            var venues = await query
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(
                venues.Select(MapToResponse).ToList(),
                "Venues retrieved successfully.");
        }

        public async Task<ApiResponse<VenueResponseDto>> GetVenueForAdminByIdAsync(int venueId)
        {
            if (venueId <= 0)
            {
                return ApiResponse<VenueResponseDto>.Fail("Venue not found.");
            }

            var venue = await _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
                .FirstOrDefaultAsync(v => v.Id == venueId);

            if (venue is null)
            {
                return ApiResponse<VenueResponseDto>.Fail("Venue not found.");
            }

            return ApiResponse<VenueResponseDto>.Ok(MapToResponse(venue), "Venue retrieved successfully.");
        }

        public async Task<ApiResponse<VenueResponseDto>> UpdateVenueStatusAsync(
            int venueId,
            UpdateVenueStatusRequestDto request)
        {
            if (venueId <= 0)
            {
                return ApiResponse<VenueResponseDto>.Fail("Venue not found.");
            }

            if (!Enum.IsDefined(request.Status))
            {
                return ApiResponse<VenueResponseDto>.Fail("Venue status is invalid.");
            }

            var venue = await _dbContext.Venues
                .Include(v => v.Images)
                .Include(v => v.VenueAmenities)
                    .ThenInclude(va => va.Amenity)
                .Include(v => v.OpeningHours)
                .FirstOrDefaultAsync(v => v.Id == venueId);

            if (venue is null)
            {
                return ApiResponse<VenueResponseDto>.Fail("Venue not found.");
            }

            if (!_allowedTransitions.TryGetValue(venue.Status, out var allowed) ||
                !allowed.Contains(request.Status))
            {
                var allowedStatusList = allowed != null
                    ? string.Join(", ", allowed)
                    : "none";
                return ApiResponse<VenueResponseDto>.Fail(
                    $"Cannot transition venue from '{venue.Status}' to '{request.Status}'. " +
                    $"Allowed transitions from '{venue.Status}': [{allowedStatusList}].");
            }

            venue.Status = request.Status;
            venue.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<VenueResponseDto>.Ok(
                MapToResponse(venue),
                $"Venue status changed to '{request.Status}' successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<VenueChangeRequestResponseDto>>> GetVenueChangeRequestsForAdminAsync(
            VenueChangeRequestStatus? status = null)
        {
            if (status.HasValue && !Enum.IsDefined(status.Value))
            {
                return ApiResponse<IReadOnlyCollection<VenueChangeRequestResponseDto>>.Fail("Change request status is invalid.");
            }

            var query = _dbContext.VenueChangeRequests.AsNoTracking();
            if (status.HasValue)
                query = query.Where(item => item.Status == status.Value);

            var requests = await query
                .OrderByDescending(item => item.CreatedAt)
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<VenueChangeRequestResponseDto>>.Ok(
                requests.Select(MapToChangeRequestResponse).ToList(),
                "Venue change requests retrieved successfully.");
        }

        public async Task<ApiResponse<VenueChangeRequestResponseDto>> UpdateVenueChangeRequestStatusAsync(
            int changeRequestId,
            UpdateVenueChangeRequestStatusRequestDto request)
        {
            if (changeRequestId <= 0)
                return ApiResponse<VenueChangeRequestResponseDto>.Fail("Venue change request not found.");

            if (request.Status is not (VenueChangeRequestStatus.Approved or VenueChangeRequestStatus.Rejected))
                return ApiResponse<VenueChangeRequestResponseDto>.Fail("Change request status must be Approved or Rejected.");

            var changeRequest = await _dbContext.VenueChangeRequests
                .Include(item => item.Venue)
                .FirstOrDefaultAsync(item => item.Id == changeRequestId);

            if (changeRequest is null)
                return ApiResponse<VenueChangeRequestResponseDto>.Fail("Venue change request not found.");

            if (changeRequest.Status != VenueChangeRequestStatus.Pending)
                return ApiResponse<VenueChangeRequestResponseDto>.Fail("Venue change request has already been processed.");

            if (request.Status == VenueChangeRequestStatus.Approved)
            {
                changeRequest.Venue.Name = changeRequest.Name;
                changeRequest.Venue.Address = changeRequest.Address;
                changeRequest.Venue.Latitude = changeRequest.Latitude;
                changeRequest.Venue.Longitude = changeRequest.Longitude;
                changeRequest.Venue.UpdatedAt = DateTimeOffset.Now;
            }

            changeRequest.Status = request.Status;
            changeRequest.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<VenueChangeRequestResponseDto>.Ok(
                MapToChangeRequestResponse(changeRequest),
                request.Status == VenueChangeRequestStatus.Approved
                    ? "Venue change request approved successfully."
                    : "Venue change request rejected successfully.");
        }

        public async Task<ApiResponse<VenueImageDto>> AddImageAsync(int userId, int venueId, AddVenueImageRequestDto request)
        {
            var ownerProfile = await FindCourtOwnerProfileAsync(userId);
            if (ownerProfile is null) return ApiResponse<VenueImageDto>.Fail("Court owner profile not found.");

            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.CourtOwnerProfileId == ownerProfile.Id);
            if (venue is null) return ApiResponse<VenueImageDto>.Fail("Venue not found.");

            var imageUrl = NormalizeOptional(request.ImageUrl);
            if (imageUrl is null)
            {
                return ApiResponse<VenueImageDto>.Fail("ImageUrl is required.");
            }

            if (request.IsCover)
            {
                var existingCover = await _dbContext.VenueImages.FirstOrDefaultAsync(i => i.VenueId == venueId && i.IsCover);
                if (existingCover != null) existingCover.IsCover = false;
            }

            var image = new VenueImage { VenueId = venueId, ImageUrl = imageUrl, IsCover = request.IsCover };
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
                Venue = MapToResponse(f.Venue),
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

        private static VenueAvailabilityCourtDto BuildAvailabilityCourt(
            Court court,
            DateTimeOffset dayStart,
            VenueOpeningHour? openingHour,
            IReadOnlyCollection<Booking> bookings,
            IReadOnlyCollection<Match> matches,
            IReadOnlyCollection<CourtSchedule> schedules,
            IReadOnlyCollection<PricingRule> pricingRules)
        {
            var slots = Enumerable.Range(0, 48).Select(index =>
            {
                var startAt = dayStart.AddMinutes(index * 30);
                var endAt = startAt.AddMinutes(30);
                return new VenueAvailabilitySlotDto
                {
                    StartAt = startAt,
                    EndAt = endAt,
                    Status = GetSlotStatus(court, startAt, endAt, openingHour, bookings, matches, schedules),
                    EstimatedPrice = GetSlotPrice(court.Id, startAt, endAt, pricingRules)
                };
            }).ToList();

            for (var index = 0; index < slots.Count - 1; index++)
            {
                slots[index].CanStartBooking =
                    slots[index].Status == "Available" && slots[index + 1].Status == "Available";
            }

            return new VenueAvailabilityCourtDto
            {
                Id = court.Id,
                Name = court.Name,
                SportId = court.SportId,
                SportName = court.Sport.Name,
                Slots = slots
            };
        }

        private static string GetSlotStatus(
            Court court,
            DateTimeOffset startAt,
            DateTimeOffset endAt,
            VenueOpeningHour? openingHour,
            IReadOnlyCollection<Booking> bookings,
            IReadOnlyCollection<Match> matches,
            IReadOnlyCollection<CourtSchedule> schedules)
        {
            if (court.Status == CourtStatus.Inactive ||
                openingHour?.IsClosed == true ||
                (openingHour is not null && (!openingHour.OpenTime.HasValue || !openingHour.CloseTime.HasValue ||
                    startAt.TimeOfDay < openingHour.OpenTime || endAt.TimeOfDay > openingHour.CloseTime)))
                return "Closed";

            if (court.Status == CourtStatus.Maintenance ||
                schedules.Any(s => s.CourtId == court.Id && s.StartAt < endAt && s.EndAt > startAt))
                return "Maintenance";

            if (bookings.Any(b => b.CourtId == court.Id && b.Status == BookingStatus.Pending && b.StartAt < endAt && b.EndAt > startAt))
                return "Held";

            if (bookings.Any(b => b.CourtId == court.Id && b.Status == BookingStatus.Confirmed && b.StartAt < endAt && b.EndAt > startAt) ||
                matches.Any(m => m.CourtId == court.Id && m.StartAt < endAt && m.EndAt > startAt))
                return "Booked";

            return "Available";
        }

        private static decimal? GetSlotPrice(int courtId, DateTimeOffset startAt, DateTimeOffset endAt, IReadOnlyCollection<PricingRule> pricingRules)
        {
            var rule = pricingRules
                .Where(r => r.CourtId == courtId && r.StartTime <= startAt.TimeOfDay && r.EndTime >= endAt.TimeOfDay)
                .OrderByDescending(r => r.EffectiveFrom)
                .ThenByDescending(r => r.StartTime)
                .FirstOrDefault();

            return rule is null ? null : Math.Round(rule.PricePerHour / 2, 2);
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
                }).ToList() ?? [],
                Amenities = venue.VenueAmenities?.Select(va => new PlayCourt.Application.DTOs.Amenities.AmenityDto
                {
                    Id = va.Amenity.Id,
                    Name = va.Amenity.Name
                }).ToList() ?? [],
                OpeningHours = venue.OpeningHours?.Select(oh => new VenueOpeningHourDto
                {
                    Id = oh.Id,
                    VenueId = oh.VenueId,
                    DayOfWeek = oh.DayOfWeek,
                    OpenTime = oh.OpenTime,
                    CloseTime = oh.CloseTime,
                    IsClosed = oh.IsClosed
                }).ToList() ?? []
            };
        }

        private static VenueChangeRequestResponseDto MapToChangeRequestResponse(VenueChangeRequest changeRequest)
        {
            return new VenueChangeRequestResponseDto
            {
                Id = changeRequest.Id,
                VenueId = changeRequest.VenueId,
                Name = changeRequest.Name,
                Address = changeRequest.Address,
                Latitude = changeRequest.Latitude,
                Longitude = changeRequest.Longitude,
                Status = changeRequest.Status.ToString(),
                CreatedAt = changeRequest.CreatedAt,
                UpdatedAt = changeRequest.UpdatedAt
            };
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
