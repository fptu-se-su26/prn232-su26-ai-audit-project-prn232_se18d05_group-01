using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.VenueImages.
    // Bảng VenueImages lưu thư viện ảnh của Venue, có thể có một ảnh cover.
    [Table("VenueImages", Schema = "dbo")]
    public class VenueImage
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Venues(Id), ON DELETE CASCADE sẽ cấu hình trong DbContext.
        [Required]
        public int VenueId { get; set; }

        // URL ảnh.
        [Required]
        public string ImageUrl { get; set; } = default!;

        // Đánh dấu ảnh bìa.
        [Required]
        public bool IsCover { get; set; } = false;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public Venue Venue { get; set; } = default!;
    }
}
