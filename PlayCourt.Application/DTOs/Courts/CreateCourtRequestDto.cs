using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Courts
{
    public class CreateCourtRequestDto
    {
        // Id môn thể thao cho sân này. Phải là SportId hợp lệ trong DB.
        [Required]
        public int SportId { get; set; }

        // Tên sân, ví dụ: Sân 1, Sân A. Tối đa 100 ký tự.
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // true = sân trong nhà, false = sân ngoài trời.
        [Required]
        public bool Indoor { get; set; }
    }
}
