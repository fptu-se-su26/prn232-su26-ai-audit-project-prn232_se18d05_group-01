using System.ComponentModel.DataAnnotations;

namespace PlayCourt.Application.DTOs
{
    public class CreateReviewRequestDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(1.0, 5.0, ErrorMessage = "Rating must be between 1.0 and 5.0.")]
        public decimal Rating { get; set; }

        public string? ReviewText { get; set; }

        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateReviewRequestDto
    {
        [Required]
        [Range(1.0, 5.0, ErrorMessage = "Rating must be between 1.0 and 5.0.")]
        public decimal Rating { get; set; }

        public string? ReviewText { get; set; }
    }

    public class AddReviewImageRequestDto
    {
        [Required]
        [Url]
        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, 100)]
        public short DisplayOrder { get; set; }
    }

    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string? PlayerAvatar { get; set; }
        public int BookingId { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public int CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string? ReviewText { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public List<ReviewImageDto> Images { get; set; } = [];
    }

    public class ReviewImageDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public short DisplayOrder { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class ReviewStatsDto
    {
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<decimal, int> RatingDistribution { get; set; } = [];
    }
}
