using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    [Table("RefreshTokens", Schema = "dbo")]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(64)]
        public string TokenHash { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? RevokedAt { get; set; }

        [MaxLength(64)]
        public string? ReplacedByTokenHash { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; } = default!;
    }
}
