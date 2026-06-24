namespace PlayCourt.Application.DTOs.CourtOwners
{
    public sealed class CourtOwnerListItemDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        public string VerificationStatus { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
    }
}
