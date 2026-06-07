using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.PricingRules;

namespace PlayCourt.Application.Interfaces
{
    public interface IPricingRuleService
    {
        // Lấy tất cả pricing rules của một court, sort theo DayOfWeek → StartTime.
        Task<ApiResponse<List<PricingRuleDto>>> GetByCourtAsync(int courtId);

        // Tạo pricing rule mới. currentUserId dùng để verify CourtOwner ownership.
        Task<ApiResponse<PricingRuleDto>> CreateAsync(int courtId, int currentUserId, CreatePricingRuleRequestDto request);

        // Cập nhật pricing rule. currentUserId dùng để verify CourtOwner ownership.
        Task<ApiResponse<PricingRuleDto>> UpdateAsync(int id, int currentUserId, UpdatePricingRuleRequestDto request);

        // Xóa pricing rule (hard-delete). currentUserId dùng để verify CourtOwner ownership.
        Task<ApiResponse<object>> DeleteAsync(int id, int currentUserId);
    }
}
