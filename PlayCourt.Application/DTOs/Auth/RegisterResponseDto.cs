namespace PlayCourt.Application.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public string? BusinessName { get; set; }
    }
}
