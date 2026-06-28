using System.ComponentModel.DataAnnotations;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.DTOs.Bookings
{
    public sealed class CreateBookingRequestDto
    {
        [Required]
        public int CourtId { get; set; }

        [Required]
        public DateTimeOffset StartAt { get; set; }

        [Required]
        public DateTimeOffset EndAt { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }

    public sealed class BookingQueryDto
    {
        public BookingStatus? Status { get; set; }

        public DateTimeOffset? From { get; set; }

        public DateTimeOffset? To { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }

    public sealed class BookingAvailabilityRequestDto
    {
        [Required]
        public DateTimeOffset StartAt { get; set; }

        [Required]
        public DateTimeOffset EndAt { get; set; }
    }

    public sealed class UpdateBookingStatusRequestDto
    {
        [MaxLength(500)]
        public string? Reason { get; set; }
    }

    public sealed class BookingResponseDto
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public int VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal OwnerEarnings { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public sealed class BookingAvailabilityResponseDto
    {
        public int CourtId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public bool IsAvailable { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public string? Reason { get; set; }
    }
}
