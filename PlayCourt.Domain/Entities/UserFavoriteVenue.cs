using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.UserFavoriteVenues.
    // Bảng UserFavoriteVenues lưu danh sách sân yêu thích của từng người dùng.
    // Composite PK: (UserProfileId, VenueId) sẽ cấu hình trong DbContext.
    [Table("UserFavoriteVenues", Schema = "dbo")]
    public class UserFavoriteVenue
    {
        // FK -> dbo.UserProfiles(Id).
        [Required]
        public int UserProfileId { get; set; }

        // FK -> dbo.Venues(Id).
        [Required]
        public int VenueId { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public UserProfile UserProfile { get; set; } = default!;

        public Venue Venue { get; set; } = default!;
    }
}
