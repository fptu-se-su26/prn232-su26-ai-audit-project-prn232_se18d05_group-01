using System.ComponentModel.DataAnnotations;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.DTOs
{
    public class AddVenueStaffRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public VenueStaffRole Role { get; set; }
    }

    public class VenueStaffResponseDto
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
