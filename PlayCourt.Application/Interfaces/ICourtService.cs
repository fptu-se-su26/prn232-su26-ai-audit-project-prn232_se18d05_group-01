using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Courts;

namespace PlayCourt.Application.Interfaces
{
    public interface ICourtService
    {
        // Lấy tất cả courts thuộc một venue (filter IsDeleted = false).
        Task<ApiResponse<List<CourtDto>>> GetByVenueAsync(int venueId);

        // Lấy chi tiết một court theo id (filter IsDeleted = false).
        Task<ApiResponse<CourtDto>> GetByIdAsync(int id);

        // Tạo court mới trong venue. currentUserId dùng để verify CourtOwner ownership.
        Task<ApiResponse<CourtDto>> CreateAsync(int venueId, int currentUserId, CreateCourtRequestDto request);

        // Cập nhật thông tin court. currentUserId dùng để verify CourtOwner ownership.
        Task<ApiResponse<CourtDto>> UpdateAsync(int id, int currentUserId, UpdateCourtRequestDto request);
    }
}
