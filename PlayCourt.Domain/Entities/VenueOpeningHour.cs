using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.VenueOpeningHours.
    // Bảng VenueOpeningHours lưu giờ mở cửa theo từng ngày trong tuần của Venue.
    [Table("VenueOpeningHours", Schema = "dbo")]
    public class VenueOpeningHour
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Venues(Id).
        [Required]
        public int VenueId { get; set; }

        // 1..7.
        [Required]
        public int DayOfWeek { get; set; }

        [Column(TypeName = "time(0)")]
        public TimeSpan? OpenTime { get; set; }

        [Column(TypeName = "time(0)")]
        public TimeSpan? CloseTime { get; set; }

        [Required]
        public bool IsClosed { get; set; } = false;

        public Venue Venue { get; set; } = default!;
    }
}
