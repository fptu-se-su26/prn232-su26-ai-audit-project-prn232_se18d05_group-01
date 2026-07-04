using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    [Table("VerificationTokens", Schema = "dbo")]
    public class VerificationToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public VerificationTokenPurpose Purpose { get; set; }

        [Required]
        [MaxLength(255)]
        public string TokenHash { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? UsedAt { get; set; }

        [Required]
        public int FailedAttempts { get; set; } = 0;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; } = default!;
    }
}
