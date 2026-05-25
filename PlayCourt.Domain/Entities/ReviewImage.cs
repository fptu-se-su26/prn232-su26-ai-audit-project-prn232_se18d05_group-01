using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.ReviewImages.
    // Bảng ReviewImages lưu hình ảnh đính kèm review.
    [Table("ReviewImages", Schema = "dbo")]
    public class ReviewImage
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Reviews(Id), ON DELETE CASCADE sẽ cấu hình trong DbContext.
        [Required]
        public int ReviewId { get; set; }

        // URL ảnh review.
        [Required]
        public string ImageUrl { get; set; } = default!;

        // Thứ tự hiển thị.
        [Required]
        public short DisplayOrder { get; set; } = 0;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public Review Review { get; set; } = default!;
    }
}
