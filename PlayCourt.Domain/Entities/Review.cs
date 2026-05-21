using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Reviews.
    // Bảng Reviews lưu đánh giá sau khi booking hoàn tất.
    [Table("Reviews", Schema = "dbo")]
    public class Review
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.UserProfiles(Id). Người đánh giá.
        [Required]
        public int PlayerId { get; set; }

        // FK -> dbo.Bookings(Id). Một booking chỉ có một review.
        [Required]
        public int BookingId { get; set; }

        // Rating từ 1.0 đến 5.0, bước 0.5. Check constraint sẽ cấu hình DbContext.
        [Required]
        [Column(TypeName = "decimal(2,1)")]
        public decimal Rating { get; set; }

        public string? ReviewText { get; set; }

        // Status: 0=Visible, 1=Hidden, 2=Reported.
        [Required]
        public ReviewStatus Status { get; set; } = ReviewStatus.Visible;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public UserProfile Player { get; set; } = default!;

        public Booking Booking { get; set; } = default!;

        public ICollection<ReviewImage> Images { get; set; } = [];
    }
}
