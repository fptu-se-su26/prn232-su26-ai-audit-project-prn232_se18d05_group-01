namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class PlayerMatchCandidateDto
    {
        public int ProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? City { get; set; }
        public string SkillLevel { get; set; } = string.Empty;
        public int MatchScore { get; set; }
    }
}
