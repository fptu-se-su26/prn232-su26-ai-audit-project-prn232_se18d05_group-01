using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Domain.Entities;

namespace PlayCourt.Infrastructure.Helpers
{
    /// <summary>
    /// Shared mapper để chuyển đổi Venue entity sang VenueResponseDto.
    /// Dùng chung cho cả VenueService và AdminVenueService, tránh DRY violation.
    /// </summary>
    internal static class VenueMapper
    {
        internal static VenueResponseDto MapToResponse(Venue venue)
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
    }
}
