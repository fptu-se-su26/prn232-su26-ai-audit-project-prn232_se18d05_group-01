using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Bookings;

namespace PlayCourt.Application.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponse<BookingResponseDto>> CreateAsync(int userId, CreateBookingRequestDto request);
        Task<ApiResponse<BookingResponseDto>> GetByIdAsync(int userId, int bookingId);
        Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> GetMyBookingsAsync(int userId, BookingQueryDto query);
        Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> GetVenueBookingsAsync(int userId, int venueId, BookingQueryDto query);
        Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> GetCourtBookingsAsync(int userId, int courtId, BookingQueryDto query);
        Task<ApiResponse<BookingAvailabilityResponseDto>> CheckAvailabilityAsync(int courtId, BookingAvailabilityRequestDto request);
        Task<ApiResponse<BookingResponseDto>> CancelByPlayerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request);
        Task<ApiResponse<BookingResponseDto>> ConfirmByOwnerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request);
        Task<ApiResponse<BookingResponseDto>> RejectByOwnerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request);
        Task<ApiResponse<BookingResponseDto>> CompleteByOwnerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request);
    }
}
