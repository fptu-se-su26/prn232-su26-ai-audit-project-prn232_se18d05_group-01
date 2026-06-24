using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Venues;

public sealed class UpdateOpeningHoursRequestDto
{
    [Required]
    public List<VenueOpeningHourDto> OpeningHours { get; set; } = new();
}
