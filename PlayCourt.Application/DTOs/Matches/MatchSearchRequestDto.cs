using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class MatchSearchRequestDto
    {
        [Range(1, int.MaxValue)]
        public int? SportId { get; set; }

        [Range(0, 2)]
        public int? SkillLevel { get; set; }

        [MaxLength(255)]
        public string? Location { get; set; }

        public DateTimeOffset? StartFrom { get; set; }

        public DateTimeOffset? StartTo { get; set; }

        public bool IncludeFull { get; set; }

        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
    }
}
