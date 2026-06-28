using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class RespondMatchInvitationDto
    {
        [Required]
        [RegularExpression("^(Accepted|Declined)$", ErrorMessage = "Status must be Accepted or Declined.")]
        public string Status { get; set; } = string.Empty;
    }
}
