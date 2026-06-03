namespace PlayCourt.Application.DTOs.Users;

public class UpdateUserProfileRequestDto
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}
