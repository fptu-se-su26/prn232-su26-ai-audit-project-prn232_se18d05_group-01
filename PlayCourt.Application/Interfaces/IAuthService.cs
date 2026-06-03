using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Auth;

namespace PlayCourt.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    }
}
