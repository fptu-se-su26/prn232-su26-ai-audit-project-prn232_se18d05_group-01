using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;

namespace PlayCourt.Application.Interfaces;

public interface IAdminVenueService
{
    Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetPendingVenuesAsync();
    Task<ApiResponse<VenueResponseDto>> ApproveVenueAsync(int venueId);
    Task<ApiResponse<VenueResponseDto>> RejectVenueAsync(int venueId);
    Task<ApiResponse<VenueResponseDto>> SuspendVenueAsync(int venueId);
}
