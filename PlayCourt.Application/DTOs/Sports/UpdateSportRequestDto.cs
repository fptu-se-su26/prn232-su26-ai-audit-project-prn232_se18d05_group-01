using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Sports
{
    public class UpdateSportRequestDto
    {
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(50, ErrorMessage = "Code must not exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public short? PlayerCount { get; set; }

        public bool IsActive { get; set; }
    }
}
