using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class RespondJoinRequestDto
    {
        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be Approved or Rejected.")]
        public string Status { get; set; } = string.Empty;
    }
}
