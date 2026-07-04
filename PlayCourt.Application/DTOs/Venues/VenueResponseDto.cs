namespace PlayCourt.Application.DTOs.Venues;

public sealed class VenueResponseDto
{
    public int Id { get; set; }
    public int CourtOwnerProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Phone { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<VenueImageDto> Images { get; set; } = new List<VenueImageDto>();
    public ICollection<PlayCourt.Application.DTOs.Amenities.AmenityDto> Amenities { get; set; } = new List<PlayCourt.Application.DTOs.Amenities.AmenityDto>();
    public ICollection<VenueOpeningHourDto> OpeningHours { get; set; } = new List<VenueOpeningHourDto>();
}
