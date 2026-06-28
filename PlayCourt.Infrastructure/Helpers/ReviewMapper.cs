using PlayCourt.Application.DTOs;
using PlayCourt.Domain.Entities;

namespace PlayCourt.Infrastructure.Helpers
{
    internal static class ReviewMapper
    {
        internal static ReviewResponseDto MapToResponse(Review review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                PlayerId = review.PlayerId,
                PlayerName = review.Player?.FullName ?? string.Empty,
                PlayerAvatar = review.Player?.AvatarUrl,
                BookingId = review.BookingId,
                VenueId = review.Booking?.Court?.VenueId ?? 0,
                VenueName = review.Booking?.Court?.Venue?.Name ?? string.Empty,
                CourtId = review.Booking?.CourtId ?? 0,
                CourtName = review.Booking?.Court?.Name ?? string.Empty,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                Status = review.Status.ToString(),
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Images = review.Images?.Select(i => new ReviewImageDto
                {
                    Id = i.Id,
                    ReviewId = i.ReviewId,
                    ImageUrl = i.ImageUrl,
                    DisplayOrder = i.DisplayOrder,
                    CreatedAt = i.CreatedAt
                }).ToList() ?? []
            };
        }
    }
}
