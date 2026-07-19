namespace PlayCourt.Application.DTOs.CourtOwners
{
    /// <summary>
    /// DTO trả về cho Owner xem hồ sơ của chính mình (GET /api/court-owners/me).
    /// </summary>
    public sealed class CourtOwnerProfileResponseDto
    {
        public int Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? BusinessLicenseNo { get; set; }
        public string? TaxCode { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessLicenseDocumentUrl { get; set; }
        public string VerificationStatus { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
