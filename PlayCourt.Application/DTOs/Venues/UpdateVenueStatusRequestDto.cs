using System.ComponentModel.DataAnnotations;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.DTOs.Venues;

public sealed class UpdateVenueStatusRequestDto
{
    [Required]
    public VenueStatus Status { get; set; }
}
