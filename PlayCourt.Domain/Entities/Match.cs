using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Matches.
    // Bảng Matches lưu bài đăng tìm người chơi / ghép trận.
    [Table("Matches", Schema = "dbo")]
    public class Match
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.UserProfiles(Id). Người tạo trận.
        [Required]
        public int HostId { get; set; }

        // FK -> dbo.Sports(Id).
        [Required]
        public int SportId { get; set; }

        // FK -> dbo.Courts(Id), nullable nếu trận chưa gắn sân cụ thể.
        public int? CourtId { get; set; }

        [MaxLength(500)]
        public string? LocationDescription { get; set; }

        [Required]
        public DateTimeOffset StartAt { get; set; }

        [Required]
        public DateTimeOffset EndAt { get; set; }

        // Trình độ tối thiểu: null hoặc 0/1/2.
        public SkillLevel? RequiredSkillLevelMin { get; set; }

        // Trình độ tối đa: null hoặc 0/1/2.
        public SkillLevel? RequiredSkillLevelMax { get; set; }

        [Required]
        public short MaxParticipants { get; set; }

        public string? CostDescription { get; set; }

        public string? Description { get; set; }

        // Status: 0=Open, 1=Full, 2=Cancelled, 3=Completed.
        [Required]
        public MatchStatus Status { get; set; } = MatchStatus.Open;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public UserProfile Host { get; set; } = default!;

        public Sport Sport { get; set; } = default!;

        public Court? Court { get; set; }

        public ICollection<MatchParticipant> Participants { get; set; } = [];

        public ICollection<MatchJoinRequest> JoinRequests { get; set; } = [];

        public ICollection<MatchInvitation> Invitations { get; set; } = [];
    }
}
