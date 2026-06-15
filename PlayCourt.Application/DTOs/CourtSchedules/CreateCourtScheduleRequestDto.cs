using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.CourtSchedules
{
    public class CreateCourtScheduleRequestDto
    {
        [Required]
        public DateTimeOffset StartAt { get; set; }

        [Required]
        public DateTimeOffset EndAt { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }
    }
}
