namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class MatchDetailResponseDto
    {
        public MatchResponseDto Match { get; set; } = new();
        public List<MatchParticipantDto> Participants { get; set; } = [];
    }
}
