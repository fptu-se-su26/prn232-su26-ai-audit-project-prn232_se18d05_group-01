using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Venues.
    // Bảng Venues lưu thông tin cụm sân/cơ sở thể thao của chủ sân.
    [Table("Venues", Schema = "dbo")]
    public class Venue
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.CourtOwnerProfiles(Id).
        [Required]
        public int CourtOwnerProfileId { get; set; }

        // Tên cơ sở/sân.
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = default!;

        // Mô tả cơ sở.
        public string? Description { get; set; }

        // Địa chỉ cơ sở.
        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = default!;

        // Vĩ độ.
        [Column(TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }

        // Kinh độ.
        [Column(TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Deprecated/display-only. Nguồn đúng là VenueOpeningHours.
        [Column(TypeName = "time(0)")]
        public TimeSpan? OpenTime { get; set; }

        // Deprecated/display-only. Nguồn đúng là VenueOpeningHours.
        [Column(TypeName = "time(0)")]
        public TimeSpan? CloseTime { get; set; }

        // Status: 0=Pending, 1=Approved, 2=Rejected, 3=Suspended.
        [Required]
        public VenueStatus Status { get; set; } = VenueStatus.Pending;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public CourtOwnerProfile CourtOwnerProfile { get; set; } = default!;

        public ICollection<Court> Courts { get; set; } = [];

        public ICollection<VenueImage> Images { get; set; } = [];

        public ICollection<VenueAmenity> VenueAmenities { get; set; } = [];

        public ICollection<UserFavoriteVenue> FavoritedByUsers { get; set; } = [];

        public ICollection<VenueOpeningHour> OpeningHours { get; set; } = [];

        public ICollection<VenueStaff> Staffs { get; set; } = [];
    }
}
