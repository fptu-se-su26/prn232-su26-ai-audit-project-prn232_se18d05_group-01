using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.VenueStaffs.
    // Bảng VenueStaffs lưu tài khoản nhân viên vận hành của từng Venue.
    [Table("VenueStaffs", Schema = "dbo")]
    public class VenueStaff
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Venues(Id).
        [Required]
        public int VenueId { get; set; }

        // FK -> dbo.Users(Id).
        [Required]
        public int UserId { get; set; }

        // Role: 0=Manager, 1=Receptionist, 2=Accountant.
        [Required]
        public VenueStaffRole Role { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public Venue Venue { get; set; } = default!;

        public User User { get; set; } = default!;
    }
}
