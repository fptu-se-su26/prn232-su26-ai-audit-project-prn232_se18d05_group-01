using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;

namespace PlayCourt.Application.Interfaces;

public interface IVenueService
{
    Task<ApiResponse<VenueResponseDto>> CreateVenueAsync(
        int userId,
        CreateVenueRequestDto request);

    Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetMyVenuesAsync(int userId);

    Task<ApiResponse<VenueResponseDto>> GetVenueByIdAsync(
        int userId,
        int venueId);

    Task<ApiResponse<VenueResponseDto>> UpdateVenueAsync(
        int userId,
        int venueId,
        UpdateVenueRequestDto request);
}
