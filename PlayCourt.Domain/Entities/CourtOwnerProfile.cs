using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.CourtOwnerProfiles.
    // Bảng CourtOwnerProfiles lưu thông tin kinh doanh/KYC của chủ sân.
    [Table("CourtOwnerProfiles", Schema = "dbo")]
    public class CourtOwnerProfile
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.UserProfiles(Id). Một profile có thể là chủ sân.
        [Required]
        public int UserProfileId { get; set; }

        // Tên doanh nghiệp / hộ kinh doanh.
        [Required]
        [MaxLength(255)]
        public string BusinessName { get; set; } = default!;

        // Số giấy phép kinh doanh.
        [MaxLength(100)]
        public string? BusinessLicenseNo { get; set; }

        // Mã số thuế.
        [MaxLength(50)]
        public string? TaxCode { get; set; }

        // Địa chỉ kinh doanh.
        public string? BusinessAddress { get; set; }

        // VerificationStatus: 0=Pending, 1=Approved, 2=Rejected.
        [Required]
        public CourtOwnerVerificationStatus VerificationStatus { get; set; } = CourtOwnerVerificationStatus.Pending;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        public UserProfile UserProfile { get; set; } = default!;

        public ICollection<Venue> Venues { get; set; } = [];
    }
}
