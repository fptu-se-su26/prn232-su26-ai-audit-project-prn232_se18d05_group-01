using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Sports;

namespace PlayCourt.Application.Interfaces
{
    public interface ISportService
    {
        Task<ApiResponse<List<SportDto>>> GetAllAsync(bool? isActive = null);

        Task<ApiResponse<SportDto>> GetByIdAsync(int id);

        Task<ApiResponse<SportDto>> CreateAsync(CreateSportRequestDto request);

        Task<ApiResponse<SportDto>> UpdateAsync(int id, UpdateSportRequestDto request);

        Task<ApiResponse<SportDto>> ToggleActiveAsync(int id);
    }
}
