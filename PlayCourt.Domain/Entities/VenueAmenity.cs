using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.VenueAmenities.
    // Bảng VenueAmenities là bảng nối nhiều-nhiều giữa Venue và Amenity.
    // Composite PK: (VenueId, AmenityId) sẽ cấu hình trong DbContext.
    [Table("VenueAmenities", Schema = "dbo")]
    public class VenueAmenity
    {
        // FK -> dbo.Venues(Id).
        [Required]
        public int VenueId { get; set; }

        // FK -> dbo.Amenities(Id).
        [Required]
        public int AmenityId { get; set; }

        public Venue Venue { get; set; } = default!;

        public Amenity Amenity { get; set; } = default!;
    }
}
