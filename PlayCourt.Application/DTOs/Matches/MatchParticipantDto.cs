namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class MatchParticipantDto
    {
        public int ProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? SkillLevel { get; set; }
        public bool IsHost { get; set; }
        public DateTimeOffset JoinedAt { get; set; }
    }
}
