using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Venues;

public sealed class UpdateOpeningHoursRequestDto
{
    [Required]
    public List<OpeningHourRequestDto> OpeningHours { get; set; } = new();
}
