using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Notifications.
    // Bảng Notifications lưu thông báo trong hệ thống cho user.
    [Table("Notifications", Schema = "dbo")]
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Users(Id). Người nhận thông báo.
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = default!;

        public string? Content { get; set; }

        // Type enum định nghĩa ở application code.
        [Required]
        public NotificationType Type { get; set; }

        // ReferenceType enum định nghĩa ở application code.
        public NotificationReferenceType? ReferenceType { get; set; }

        // Id tham chiếu tới Booking/Match/Payment... tùy ReferenceType.
        public int? ReferenceId { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public User User { get; set; } = default!;
    }
}
