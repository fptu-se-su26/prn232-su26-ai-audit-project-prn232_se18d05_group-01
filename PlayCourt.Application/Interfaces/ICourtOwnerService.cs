using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtOwners;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.Interfaces
{
    public interface ICourtOwnerService
    {
        Task<ApiResponse<List<CourtOwnerListItemDto>>> GetAllAsync(
            CourtOwnerVerificationStatus? verificationStatus = null);

        Task<ApiResponse<CourtOwnerDetailDto>> GetByIdAsync(int id);

        Task<ApiResponse<CourtOwnerDetailDto>> UpdateVerificationStatusAsync(
            int id,
            UpdateCourtOwnerVerificationStatusRequestDto request);
    }
}
