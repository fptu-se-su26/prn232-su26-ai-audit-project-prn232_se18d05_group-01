using PlayCourt.Application.DTOs.Auth;
using PlayCourt.Domain.Entities;

namespace PlayCourt.Application.Interfaces
{
    public interface IJwtTokenService
    {
        JwtTokenResult GenerateAccessToken(User user, UserProfile? profile);
    }
}
