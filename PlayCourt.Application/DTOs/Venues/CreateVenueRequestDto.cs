using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Venues;

public sealed class CreateVenueRequestDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(255, ErrorMessage = "Name must not exceed 255 characters.")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [MaxLength(500, ErrorMessage = "Address must not exceed 500 characters.")]
    public string? Address { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public decimal? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public decimal? Longitude { get; set; }

    [MaxLength(20, ErrorMessage = "Phone must not exceed 20 characters.")]
    public string? Phone { get; set; }

    public TimeSpan? OpenTime { get; set; }

    public TimeSpan? CloseTime { get; set; }
}
