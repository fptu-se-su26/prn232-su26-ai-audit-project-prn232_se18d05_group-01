namespace PlayCourt.Application.DTOs.CourtSchedules
{
    public class CourtScheduleDto
    {
        public int Id { get; set; }

        public int CourtId { get; set; }

        public DateTimeOffset StartAt { get; set; }

        public DateTimeOffset EndAt { get; set; }

        public string? Reason { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
