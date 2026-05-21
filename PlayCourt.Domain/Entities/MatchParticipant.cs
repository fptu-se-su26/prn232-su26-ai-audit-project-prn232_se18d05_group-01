using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.MatchParticipants.
    // Bảng MatchParticipants lưu người tham gia trong một match.
    [Table("MatchParticipants", Schema = "dbo")]
    public class MatchParticipant
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Matches(Id), ON DELETE CASCADE sẽ cấu hình trong DbContext.
        [Required]
        public int MatchId { get; set; }

        // FK -> dbo.UserProfiles(Id).
        [Required]
        public int PlayerId { get; set; }

        [Required]
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.Now;

        // true nếu là host.
        [Required]
        public bool IsHost { get; set; } = false;

        public Match Match { get; set; } = default!;

        public UserProfile Player { get; set; } = default!;
    }
}
