using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Amenities.
    // Bảng Amenities lưu danh mục tiện ích: wifi, parking, locker...
    [Table("Amenities", Schema = "dbo")]
    public class Amenity
    {
        [Key]
        public int Id { get; set; }

        // Tên tiện ích. Unique sẽ cấu hình trong DbContext.
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        public ICollection<VenueAmenity> VenueAmenities { get; set; } = [];
    }
}
