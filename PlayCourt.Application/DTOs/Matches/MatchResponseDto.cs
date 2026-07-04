namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class MatchResponseDto
    {
        public int Id { get; set; }
        public int HostProfileId { get; set; }
        public string HostName { get; set; } = string.Empty;
        public string? HostAvatarUrl { get; set; }
        public int SportId { get; set; }
        public string SportCode { get; set; } = string.Empty;
        public string SportName { get; set; } = string.Empty;
        public int? CourtId { get; set; }
        public string? CourtName { get; set; }
        public string? VenueName { get; set; }
        public string? LocationDescription { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public string? RequiredSkillLevelMin { get; set; }
        public string? RequiredSkillLevelMax { get; set; }
        public short MaxParticipants { get; set; }
        public int ParticipantCount { get; set; }
        public int AvailableSlots { get; set; }
        public string? CostDescription { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsHost { get; set; }
        public bool IsParticipant { get; set; }
        public string? MyJoinRequestStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
