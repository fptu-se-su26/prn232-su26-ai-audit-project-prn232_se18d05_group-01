using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.MatchInvitations.
    // Bảng MatchInvitations lưu lời mời người chơi tham gia match.
    [Table("MatchInvitations", Schema = "dbo")]
    public class MatchInvitation
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Matches(Id), ON DELETE CASCADE sẽ cấu hình trong DbContext.
        [Required]
        public int MatchId { get; set; }

        // FK -> dbo.UserProfiles(Id). Người gửi lời mời.
        [Required]
        public int InviterId { get; set; }

        // FK -> dbo.UserProfiles(Id). Người được mời.
        [Required]
        public int InviteeId { get; set; }

        // Status: 0=Pending, 1=Accepted, 2=Declined, 3=Cancelled.
        [Required]
        public MatchInvitationStatus Status { get; set; } = MatchInvitationStatus.Pending;

        [MaxLength(500)]
        public string? Message { get; set; }

        [Required]
        public DateTimeOffset InvitedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? RespondedAt { get; set; }

        public Match Match { get; set; } = default!;

        public UserProfile Inviter { get; set; } = default!;

        public UserProfile Invitee { get; set; } = default!;
    }
}
