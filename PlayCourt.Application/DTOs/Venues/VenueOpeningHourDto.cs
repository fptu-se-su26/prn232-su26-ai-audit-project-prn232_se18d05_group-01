namespace PlayCourt.Application.DTOs.Venues;

public sealed class VenueOpeningHourDto
{
    public int Id { get; set; }
    public int VenueId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}
