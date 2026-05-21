using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.BookingStatusHistories.
    // Bảng BookingStatusHistories lưu lịch sử thay đổi trạng thái booking.
    [Table("BookingStatusHistories", Schema = "dbo")]
    public class BookingStatusHistory
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Bookings(Id).
        [Required]
        public int BookingId { get; set; }

        // Trạng thái cũ, có thể null nếu là lần tạo đầu tiên.
        public BookingStatus? OldStatus { get; set; }

        // Trạng thái mới.
        [Required]
        public BookingStatus NewStatus { get; set; }

        // FK -> dbo.Users(Id), user thực hiện thay đổi, có thể null nếu hệ thống tự đổi.
        public int? ChangedByUserId { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public Booking Booking { get; set; } = default!;

        public User? ChangedByUser { get; set; }
    }
}
