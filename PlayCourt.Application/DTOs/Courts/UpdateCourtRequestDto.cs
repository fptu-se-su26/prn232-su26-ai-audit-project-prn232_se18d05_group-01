using System.ComponentModel.DataAnnotations;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.DTOs.Courts
{
    public class UpdateCourtRequestDto
    {
        // Id môn thể thao cho sân này. Phải là SportId hợp lệ trong DB.
        [Required]
        public int SportId { get; set; }

        // Tên sân. Tối đa 100 ký tự.
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // true = sân trong nhà, false = sân ngoài trời.
        [Required]
        public bool Indoor { get; set; }

        // Trạng thái sân: Available = 0, Maintenance = 1, Inactive = 2.
        [Required]
        public CourtStatus Status { get; set; }
    }
}
