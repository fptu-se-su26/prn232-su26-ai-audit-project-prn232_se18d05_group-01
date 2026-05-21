using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.MatchJoinRequests.
    // Bảng MatchJoinRequests lưu yêu cầu xin tham gia match.
    [Table("MatchJoinRequests", Schema = "dbo")]
    public class MatchJoinRequest
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Matches(Id), ON DELETE CASCADE sẽ cấu hình trong DbContext.
        [Required]
        public int MatchId { get; set; }

        // FK -> dbo.UserProfiles(Id).
        [Required]
        public int PlayerId { get; set; }

        // Status: 0=Pending, 1=Approved, 2=Rejected.
        [Required]
        public MatchJoinRequestStatus Status { get; set; } = MatchJoinRequestStatus.Pending;

        [Required]
        public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? RespondedAt { get; set; }

        public Match Match { get; set; } = default!;

        public UserProfile Player { get; set; } = default!;
    }
}
