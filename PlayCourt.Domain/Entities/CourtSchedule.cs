using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.CourtSchedules.
    // Bảng CourtSchedules lưu lịch khóa sân/bảo trì để không cho booking trùng.
    [Table("CourtSchedules", Schema = "dbo")]
    public class CourtSchedule
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Courts(Id), ON DELETE CASCADE sẽ cấu hình trong DbContext.
        [Required]
        public int CourtId { get; set; }

        // Thời gian bắt đầu khóa/bảo trì.
        [Required]
        public DateTimeOffset StartAt { get; set; }

        // Thời gian kết thúc khóa/bảo trì. Check StartAt < EndAt cấu hình DbContext.
        [Required]
        public DateTimeOffset EndAt { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public Court Court { get; set; } = default!;
    }
}
