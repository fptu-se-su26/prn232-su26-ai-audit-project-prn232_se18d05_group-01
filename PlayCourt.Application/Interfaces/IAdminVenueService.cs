using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;

namespace PlayCourt.Application.Interfaces;

public interface IAdminVenueService
{
    /// <summary>Lấy danh sách venues kèm filter trạng thái (null = tất cả).</summary>
    Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetAllVenuesAsync(string? status = null);

    /// <summary>Lấy chi tiết một venue theo ID (không giới hạn trạng thái).</summary>
    Task<ApiResponse<VenueResponseDto>> GetVenueByIdAsync(int venueId);

    // State transitions — mỗi action chỉ chấp nhận trạng thái hiện tại hợp lệ
    Task<ApiResponse<VenueResponseDto>> ApproveVenueAsync(int venueId);
    Task<ApiResponse<VenueResponseDto>> RejectVenueAsync(int venueId);
    Task<ApiResponse<VenueResponseDto>> SuspendVenueAsync(int venueId);

    /// <summary>Khôi phục venue từ Suspended về Approved.</summary>
    Task<ApiResponse<VenueResponseDto>> RestoreVenueAsync(int venueId);
}
