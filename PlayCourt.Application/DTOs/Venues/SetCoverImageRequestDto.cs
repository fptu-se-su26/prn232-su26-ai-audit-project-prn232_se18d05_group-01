using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Venues;

public sealed class SetCoverImageRequestDto
{
    [Required]
    public int ImageId { get; set; }
}
