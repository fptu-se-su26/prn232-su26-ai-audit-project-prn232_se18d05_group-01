namespace PlayCourt.Application.DTOs.Venues;

public sealed class VenueImageDto
{
    public int Id { get; set; }
    public int VenueId { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsCover { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
