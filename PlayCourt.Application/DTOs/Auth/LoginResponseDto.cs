namespace PlayCourt.Application.DTOs.Auth
{
    public sealed class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public DateTimeOffset RefreshTokenExpiresAt { get; set; }
        public LoginUserDto User { get; set; } = default!;
    }

    public sealed class LoginUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
    }
}
