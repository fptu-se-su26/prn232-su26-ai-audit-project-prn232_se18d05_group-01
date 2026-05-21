using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.PlayerSports.
    // Bảng PlayerSports lưu quan hệ nhiều-nhiều giữa UserProfile và Sport, kèm trình độ.
    [Table("PlayerSports", Schema = "dbo")]
    public class PlayerSport
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.UserProfiles(Id).
        [Required]
        public int UserProfileId { get; set; }

        // FK -> dbo.Sports(Id).
        [Required]
        public int SportId { get; set; }

        // SkillLevel: 0=Beginner, 1=Intermediate, 2=Advanced.
        [Required]
        public SkillLevel SkillLevel { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public UserProfile UserProfile { get; set; } = default!;

        public Sport Sport { get; set; } = default!;
    }
}
