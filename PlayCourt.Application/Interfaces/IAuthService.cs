using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Auth;

namespace PlayCourt.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<object>> VerifyEmailAsync(VerifyEmailRequestDto request);
        Task<ApiResponse<object>> ResendVerifyEmailAsync(ResendVerifyEmailRequestDto request);
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponse<object>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
        Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<ApiResponse<object>> LogoutAsync(LogoutRequestDto request);
    }
}
