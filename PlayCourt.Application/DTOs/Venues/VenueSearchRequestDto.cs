namespace PlayCourt.Application.DTOs.Venues;

public sealed class VenueSearchRequestDto
{
    public string? Keyword { get; set; }
    public int? SportId { get; set; }
    public bool? IsOpenNow { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
