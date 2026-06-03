namespace PlayCourt.Application.DTOs.Users;

public class UserProfileResponseDto
{
    public int UserId { get; set; }
    public int ProfileId { get; set; }

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public CourtOwnerProfileSummaryDto? CourtOwnerProfile { get; set; }
}
