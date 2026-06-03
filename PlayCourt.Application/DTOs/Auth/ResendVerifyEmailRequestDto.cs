using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Auth
{
    public sealed class ResendVerifyEmailRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}
