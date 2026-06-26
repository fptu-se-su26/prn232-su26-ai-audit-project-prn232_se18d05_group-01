namespace PlayCourt.Application.DTOs.Venues;

public sealed class VenueStatsResponseDto
{
    public int TotalVenues { get; set; }
    public int TotalCourts { get; set; }
    public int TodayBookings { get; set; }
    // Add other fields later if needed
}
