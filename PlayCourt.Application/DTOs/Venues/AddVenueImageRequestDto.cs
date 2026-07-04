using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Venues;

public sealed class AddVenueImageRequestDto
{
    [Required(ErrorMessage = "ImageUrl is required.")]
    [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
    public string? ImageUrl { get; set; }

    public bool IsCover { get; set; }
}
