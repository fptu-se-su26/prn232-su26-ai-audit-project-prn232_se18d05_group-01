namespace PlayCourt.Application.DTOs.Users;

public class CourtOwnerProfileSummaryDto
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessLicenseNo { get; set; }
    public string? TaxCode { get; set; }
    public string? BusinessAddress { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
}
