using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Matches
{
    public sealed class CreateMatchInvitationDto
    {
        [Range(1, int.MaxValue)]
        public int InviteeProfileId { get; set; }

        [MaxLength(500)]
        public string? Message { get; set; }
    }
}
