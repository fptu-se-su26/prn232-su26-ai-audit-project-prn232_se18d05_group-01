using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.PricingRules
{
    public class UpdatePricingRuleRequestDto
    {
        // Thứ trong tuần: 1 = Monday, 2 = Tuesday, ..., 7 = Sunday.
        [Required]
        [Range(1, 7, ErrorMessage = "DayOfWeek phải từ 1 (Monday) đến 7 (Sunday).")]
        public int DayOfWeek { get; set; }

        // Giờ bắt đầu, format "HH:mm".
        [Required]
        public string StartTime { get; set; } = string.Empty;

        // Giờ kết thúc, format "HH:mm". Phải sau StartTime.
        [Required]
        public string EndTime { get; set; } = string.Empty;

        // Giá thuê mỗi giờ. Phải > 0.
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "PricePerHour phải lớn hơn 0.")]
        public decimal PricePerHour { get; set; }

        // Ngày bắt đầu hiệu lực.
        [Required]
        public DateTime EffectiveFrom { get; set; }

        // Ngày kết thúc hiệu lực. Null = không giới hạn.
        public DateTime? EffectiveTo { get; set; }
    }
}
