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

        Task<ApiResponse<List<PlayerSportResponseDto>>> GetCurrentUserSportsAsync(int userId);

        Task<ApiResponse<PlayerSportResponseDto>> AddCurrentUserSportAsync(
            int userId,
            AddPlayerSportRequestDto request);

        Task<ApiResponse<PlayerSportResponseDto>> UpdateCurrentUserSportAsync(
            int userId,
            int sportId,
            UpdatePlayerSportRequestDto request);

        Task<ApiResponse<PlayerSportResponseDto>> RemoveCurrentUserSportAsync(
            int userId,
            int sportId);
    }
}
