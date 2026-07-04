using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.Interfaces
{
    public interface IVerificationTokenService
    {
        Task<string> CreateOtpAsync(int userId, VerificationTokenPurpose purpose, int expiryMinutes = 10);
        Task<bool> VerifyOtpAsync(int userId, VerificationTokenPurpose purpose, string otp);
    }
}
