namespace PlayCourt.Application.DTOs.Venues;

public sealed class VenueAvailabilityResponseDto
{
    public DateOnly Date { get; set; }
    public VenueAvailabilityVenueDto Venue { get; set; } = new();
    public IReadOnlyCollection<VenueAvailabilityCourtDto> Courts { get; set; } = [];
}

public sealed class VenueAvailabilityVenueDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}

public sealed class VenueAvailabilityCourtDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SportId { get; set; }
    public string SportName { get; set; } = string.Empty;
    public IReadOnlyCollection<VenueAvailabilitySlotDto> Slots { get; set; } = [];
}

public sealed class VenueAvailabilitySlotDto
{
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? EstimatedPrice { get; set; }
    public bool CanStartBooking { get; set; }
}
