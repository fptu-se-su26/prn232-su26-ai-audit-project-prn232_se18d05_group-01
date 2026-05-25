using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Users.
    // Bảng Users lưu thông tin tài khoản đăng nhập, phân quyền, trạng thái và soft-delete.
    [Table("Users", Schema = "dbo")]
    public class User
    {
        // [Key]: khóa chính.
        [Key]
        public int Id { get; set; }

        // Email đăng nhập. Unique active sẽ cấu hình trong DbContext.
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = default!;

        // Số điện thoại, có thể null.
        [MaxLength(20)]
        public string? Phone { get; set; }

        // Mật khẩu đã hash, không lưu plain text.
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = default!;

        // Role: 0=Admin, 1=Player, 2=CourtOwner.
        [Required]
        public UserRole Role { get; set; }

        // Status: 0=Active, 1=Locked, 2=Inactive.
        [Required]
        public UserStatus Status { get; set; } = UserStatus.Active;

        // Đánh dấu email đã xác thực hay chưa.
        [Required]
        public bool IsEmailVerified { get; set; } = false;

        // Ngày tạo tài khoản.
        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        // Ngày cập nhật gần nhất.
        public DateTimeOffset? UpdatedAt { get; set; }

        // Soft-delete, không xóa vật lý khỏi DB.
        [Required]
        public bool IsDeleted { get; set; } = false;

        public UserProfile? UserProfile { get; set; }

        public ICollection<Notification> Notifications { get; set; } = [];

        public ICollection<Payment> Payments { get; set; } = [];

        public ICollection<VenueStaff> VenueStaffs { get; set; } = [];

        public ICollection<BookingStatusHistory> BookingStatusHistories { get; set; } = [];
    }
}
