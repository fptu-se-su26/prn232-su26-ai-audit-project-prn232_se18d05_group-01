using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Sports.
    // Bảng Sports lưu danh mục môn thể thao: badminton, football, tennis...
    [Table("Sports", Schema = "dbo")]
    public class Sport
    {
        [Key]
        public int Id { get; set; }

        // Mã môn thể thao. Unique sẽ cấu hình trong DbContext.
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = default!;

        // Tên môn thể thao.
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        // Mô tả thêm.
        public string? Description { get; set; }

        // Số người chơi, nếu môn đó có quy định.
        public short? PlayerCount { get; set; }

        // Còn hoạt động hay không.
        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public ICollection<PlayerSport> PlayerSports { get; set; } = [];

        public ICollection<Court> Courts { get; set; } = [];

        public ICollection<Match> Matches { get; set; } = [];
    }
}
