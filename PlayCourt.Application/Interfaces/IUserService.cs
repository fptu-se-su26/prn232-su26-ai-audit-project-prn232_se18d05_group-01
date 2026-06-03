using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Users;

namespace PlayCourt.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserProfileResponseDto>> GetCurrentUserProfileAsync(int userId);

        Task<ApiResponse<UserProfileResponseDto>> UpdateCurrentUserProfileAsync(
            int userId,
            UpdateUserProfileRequestDto request);
    }
}
