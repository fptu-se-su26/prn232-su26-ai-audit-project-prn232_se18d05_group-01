namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class MatchJoinRequestDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerProfileId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string? PlayerAvatarUrl { get; set; }
        public string? SkillLevel { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset RequestedAt { get; set; }
        public DateTimeOffset? RespondedAt { get; set; }
    }
}
