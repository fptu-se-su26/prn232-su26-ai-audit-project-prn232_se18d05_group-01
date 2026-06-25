namespace PlayCourt.Application.DTOs.Venues;

public sealed class FavoriteVenueResponseDto
{
    public int UserProfileId { get; set; }

    public int VenueId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public VenueResponseDto Venue { get; set; } = default!;
}
