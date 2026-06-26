namespace PlayCourt.Application.DTOs.Venues;

public sealed class OpeningHourRequestDto
{
    public int DayOfWeek { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}
