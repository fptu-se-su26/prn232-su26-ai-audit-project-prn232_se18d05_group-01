namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class MatchInvitationDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public DateTimeOffset MatchStartAt { get; set; }
        public int InviterProfileId { get; set; }
        public string InviterName { get; set; } = string.Empty;
        public int InviteeProfileId { get; set; }
        public string InviteeName { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset InvitedAt { get; set; }
        public DateTimeOffset? RespondedAt { get; set; }
    }
}
