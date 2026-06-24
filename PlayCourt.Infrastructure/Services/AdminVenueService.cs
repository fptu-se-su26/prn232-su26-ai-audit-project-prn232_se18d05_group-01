using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services;

public sealed class AdminVenueService : IAdminVenueService
{
    private readonly PlayCourtDbContext _dbContext;

    public AdminVenueService(PlayCourtDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetPendingVenuesAsync()
    {
        var venues = await _dbContext.Venues
            .AsNoTracking()
            .Include(v => v.Images)
            .Include(v => v.VenueAmenities)
                .ThenInclude(va => va.Amenity)
            .Include(v => v.OpeningHours)
            .Where(v => v.Status == VenueStatus.Pending)
            .OrderBy(v => v.CreatedAt)
            .Select(v => MapToResponse(v))
            .ToListAsync();

        return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(venues, "Pending venues retrieved successfully.");
    }

    public async Task<ApiResponse<VenueResponseDto>> ApproveVenueAsync(int venueId)
    {
        return await ChangeVenueStatusAsync(venueId, VenueStatus.Approved);
    }

    public async Task<ApiResponse<VenueResponseDto>> RejectVenueAsync(int venueId)
    {
        return await ChangeVenueStatusAsync(venueId, VenueStatus.Rejected);
    }

    public async Task<ApiResponse<VenueResponseDto>> SuspendVenueAsync(int venueId)
    {
        return await ChangeVenueStatusAsync(venueId, VenueStatus.Suspended);
    }

    private async Task<ApiResponse<VenueResponseDto>> ChangeVenueStatusAsync(int venueId, VenueStatus newStatus)
    {
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

        venue.Status = newStatus;
        venue.UpdatedAt = DateTimeOffset.Now;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<VenueResponseDto>.Ok(MapToResponse(venue), $"Venue status changed to {newStatus}.");
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
            Images = venue.Images.Select(i => new VenueImageDto
            {
                Id = i.Id,
                VenueId = i.VenueId,
                ImageUrl = i.ImageUrl,
                IsCover = i.IsCover,
                CreatedAt = i.CreatedAt
            }).ToList(),
            Amenities = venue.VenueAmenities.Select(va => new PlayCourt.Application.DTOs.Amenities.AmenityDto
            {
                Id = va.Amenity.Id,
                Name = va.Amenity.Name
            }).ToList(),
            OpeningHours = venue.OpeningHours.Select(oh => new VenueOpeningHourDto
            {
                Id = oh.Id,
                VenueId = oh.VenueId,
                DayOfWeek = oh.DayOfWeek,
                OpenTime = oh.OpenTime,
                CloseTime = oh.CloseTime,
                IsClosed = oh.IsClosed
            }).ToList()
        };
    }
}
