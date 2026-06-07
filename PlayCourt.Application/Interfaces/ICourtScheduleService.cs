using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtSchedules;

namespace PlayCourt.Application.Interfaces
{
    public interface ICourtScheduleService
    {
        Task<ApiResponse<List<CourtScheduleDto>>> GetByCourtAsync(int courtId);
        Task<ApiResponse<CourtScheduleDto>> CreateAsync(int courtId, int currentUserId, CreateCourtScheduleRequestDto request);
        Task<ApiResponse<object>> DeleteAsync(int id, int currentUserId);
    }
}
