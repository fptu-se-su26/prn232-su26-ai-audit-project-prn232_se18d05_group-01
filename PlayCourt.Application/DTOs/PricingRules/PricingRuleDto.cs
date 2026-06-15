namespace PlayCourt.Application.DTOs.PricingRules
{
    public class PricingRuleDto
    {
        public int Id { get; set; }

        public int CourtId { get; set; }

        // 1 = Monday, 2 = Tuesday, ..., 7 = Sunday.
        public int DayOfWeek { get; set; }

        // Format "HH:mm", ví dụ "08:00".
        public string StartTime { get; set; } = string.Empty;

        // Format "HH:mm", ví dụ "12:00".
        public string EndTime { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; }

        // Ngày bắt đầu hiệu lực, format "yyyy-MM-dd".
        public string EffectiveFrom { get; set; } = string.Empty;

        // Ngày kết thúc hiệu lực, null = không giới hạn.
        public string? EffectiveTo { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
