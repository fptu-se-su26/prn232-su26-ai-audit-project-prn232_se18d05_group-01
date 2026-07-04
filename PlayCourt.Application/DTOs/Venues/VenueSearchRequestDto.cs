using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs.Venues;

public sealed class VenueSearchRequestDto
{
    public string? Keyword { get; set; }

    /// <summary>Lọc theo bộ môn thể thao (ID của Sport). Null = không lọc.</summary>
    public int? SportId { get; set; }

    /// <summary>Lọc các sân đang mở cửa tại thời điểm hiện tại. Null = không lọc.</summary>
    public bool? IsOpenNow { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PageIndex phải lớn hơn hoặc bằng 1.")]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize phải từ 1 đến 100.")]
    public int PageSize { get; set; } = 10;
}
