namespace PlayCourt.Application.DTOs.Users;

public class PlayerSportResponseDto
{
    public int Id { get; set; }
    public int SportId { get; set; }
    public string SportCode { get; set; } = string.Empty;
    public string SportName { get; set; } = string.Empty;
    public string SkillLevel { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
