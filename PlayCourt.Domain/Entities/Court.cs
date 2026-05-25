using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Courts.
    // Bảng Courts lưu từng sân con thuộc một Venue và một Sport.
    [Table("Courts", Schema = "dbo")]
    public class Court
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Venues(Id).
        [Required]
        public int VenueId { get; set; }

        // FK -> dbo.Sports(Id).
        [Required]
        public int SportId { get; set; }

        // Tên sân, ví dụ: Sân 1, Sân A.
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        // true = sân trong nhà, false = sân ngoài trời.
        [Required]
        public bool Indoor { get; set; } = false;

        // Status: 0=Available, 1=Maintenance, 2=Inactive.
        [Required]
        public CourtStatus Status { get; set; } = CourtStatus.Available;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public Venue Venue { get; set; } = default!;

        public Sport Sport { get; set; } = default!;

        public ICollection<CourtSchedule> CourtSchedules { get; set; } = [];

        public ICollection<PricingRule> PricingRules { get; set; } = [];

        public ICollection<Booking> Bookings { get; set; } = [];

        public ICollection<Match> Matches { get; set; } = [];
    }
}
