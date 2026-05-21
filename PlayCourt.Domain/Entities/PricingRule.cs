using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.PricingRules.
    // Bảng PricingRules lưu giá thuê sân theo thứ trong tuần và khung giờ.
    [Table("PricingRules", Schema = "dbo")]
    public class PricingRule
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Courts(Id), ON DELETE CASCADE sẽ cấu hình trong DbContext.
        [Required]
        public int CourtId { get; set; }

        // 1..7. Check constraint sẽ cấu hình trong DbContext.
        [Required]
        public int DayOfWeek { get; set; }

        [Required]
        [Column(TypeName = "time(0)")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column(TypeName = "time(0)")]
        public TimeSpan EndTime { get; set; }

        // Giá theo giờ.
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerHour { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EffectiveFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EffectiveTo { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public Court Court { get; set; } = default!;
    }
}
