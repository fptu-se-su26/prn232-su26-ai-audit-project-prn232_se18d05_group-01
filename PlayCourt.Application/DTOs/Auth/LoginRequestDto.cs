using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Auth
{
    public sealed class LoginRequestDto
    {
        [Required(ErrorMessage = "Identifier is required")]
        public string Identifier { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
