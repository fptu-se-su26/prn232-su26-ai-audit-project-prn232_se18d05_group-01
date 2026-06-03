using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Auth
{
    public sealed class VerifyEmailRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Otp is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Otp must be 6 digits")]
        public string Otp { get; set; } = string.Empty;
    }
}
