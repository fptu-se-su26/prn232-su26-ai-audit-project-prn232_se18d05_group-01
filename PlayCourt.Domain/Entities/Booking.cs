using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Bookings.
    // Bảng Bookings lưu lịch đặt sân. VenueId được suy ra qua CourtId -> Courts -> Venues.
    [Table("Bookings", Schema = "dbo")]
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.UserProfiles(Id). Người đặt sân.
        [Required]
        public int UserProfileId { get; set; }

        // FK -> dbo.Courts(Id). Sân được đặt.
        [Required]
        public int CourtId { get; set; }

        [Required]
        public DateTimeOffset StartAt { get; set; }

        [Required]
        public DateTimeOffset EndAt { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal PlatformFee { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal OwnerEarnings { get; set; }

        // Status: 0=Pending, 1=Confirmed, 2=CancelledByUser, 3=CancelledByOwner, 4=Completed, 5=Expired.
        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public string? Note { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public UserProfile UserProfile { get; set; } = default!;

        public Court Court { get; set; } = default!;

        public ICollection<Payment> Payments { get; set; } = [];

        public ICollection<BookingStatusHistory> StatusHistories { get; set; } = [];

        public Review? Review { get; set; }
    }
}
