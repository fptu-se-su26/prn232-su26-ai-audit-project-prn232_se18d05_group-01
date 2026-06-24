using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.CourtOwners
{
    public sealed class UpdateCourtOwnerVerificationStatusRequestDto
    {
        public int VerificationStatus { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }
    }
}
