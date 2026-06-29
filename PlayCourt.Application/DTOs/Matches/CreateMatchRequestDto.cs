using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Matches
{
    public class CreateMatchRequestDto
    {
        [Range(1, int.MaxValue)]
        public int SportId { get; set; }

        [Range(1, int.MaxValue)]
        public int? CourtId { get; set; }

        [MaxLength(500)]
        public string? LocationDescription { get; set; }

        public DateTimeOffset StartAt { get; set; }

        public DateTimeOffset EndAt { get; set; }

        [Range(0, 2)]
        public int? RequiredSkillLevelMin { get; set; }

        [Range(0, 2)]
        public int? RequiredSkillLevelMax { get; set; }

        [Range(2, 100)]
        public short MaxParticipants { get; set; }

        [MaxLength(500)]
        public string? CostDescription { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }
    }
}
