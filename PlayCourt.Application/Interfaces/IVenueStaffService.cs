using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs;

namespace PlayCourt.Application.Interfaces
{
    public interface IVenueStaffService
    {
        Task<ApiResponse<VenueStaffResponseDto>> AddStaffAsync(int ownerUserId, int venueId, AddVenueStaffRequestDto request);
        Task<ApiResponse<IReadOnlyCollection<VenueStaffResponseDto>>> GetVenueStaffAsync(int userId, int venueId);
        Task<ApiResponse<object>> RemoveStaffAsync(int ownerUserId, int venueId, int staffId);
    }
}
