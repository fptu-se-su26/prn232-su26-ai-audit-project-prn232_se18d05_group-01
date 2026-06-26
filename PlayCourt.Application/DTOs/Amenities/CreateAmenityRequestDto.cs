using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Amenities;

public sealed class CreateAmenityRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
}
