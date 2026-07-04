namespace PlayCourt.Application.DTOs.CourtOwners
{
    public sealed class CourtOwnerDetailDto
    {
        public int Id { get; set; }

        public int UserProfileId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        public string? BusinessLicenseNo { get; set; }

        public string? TaxCode { get; set; }

        public string? BusinessAddress { get; set; }

        public string VerificationStatus { get; set; } = string.Empty;

        public string? RejectionReason { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
