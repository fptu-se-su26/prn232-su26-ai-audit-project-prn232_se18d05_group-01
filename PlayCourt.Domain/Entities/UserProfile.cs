using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.UserProfiles.
    // Bảng UserProfiles lưu thông tin hồ sơ 1-1 của user.
    [Table("UserProfiles", Schema = "dbo")]
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Users(Id). Một User chỉ có một UserProfile.
        [Required]
        public int UserId { get; set; }

        // Họ tên người dùng.
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = default!;

        // Ảnh đại diện.
        public string? AvatarUrl { get; set; }

        // Ngày sinh.
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        // Gender: null hoặc 0/1/2.
        public Gender? Gender { get; set; }

        // Địa chỉ chi tiết.
        public string? Address { get; set; }

        [MaxLength(255)]
        public string? City { get; set; }

        [MaxLength(255)]
        public string? Country { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        public User User { get; set; } = default!;

        public CourtOwnerProfile? CourtOwnerProfile { get; set; }

        public ICollection<PlayerSport> PlayerSports { get; set; } = [];

        public ICollection<Booking> Bookings { get; set; } = [];

        public ICollection<UserFavoriteVenue> FavoriteVenues { get; set; } = [];

        public ICollection<Review> Reviews { get; set; } = [];

        public ICollection<Match> HostedMatches { get; set; } = [];

        public ICollection<MatchParticipant> MatchParticipations { get; set; } = [];

        public ICollection<MatchJoinRequest> MatchJoinRequests { get; set; } = [];

        public ICollection<MatchInvitation> SentMatchInvitations { get; set; } = [];

        public ICollection<MatchInvitation> ReceivedMatchInvitations { get; set; } = [];
    }
}
