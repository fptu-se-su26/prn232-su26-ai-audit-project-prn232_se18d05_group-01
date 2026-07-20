using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.CourtOwners
{
    /// <summary>
    /// DTO request để Owner cập nhật hồ sơ (PUT /api/court-owners/me).
    /// </summary>
    public sealed class UpdateCourtOwnerProfileRequestDto
    {
        [MaxLength(255)]
        public string? BusinessName { get; set; }

        [MaxLength(100)]
        public string? BusinessLicenseNo { get; set; }

        [MaxLength(50)]
        public string? TaxCode { get; set; }

        [MaxLength(500)]
        public string? BusinessAddress { get; set; }

        [MaxLength(1000)]
        public string? BusinessLicenseDocumentUrl { get; set; }
    }
}
