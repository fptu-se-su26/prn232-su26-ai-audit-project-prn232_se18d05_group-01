using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void GenerateAccessToken_IncludesRoleClaim()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "playcourt-development-secret-key-change-later-123456",
                ["Jwt:Issuer"] = "PlayCourt",
                ["Jwt:Audience"] = "PlayCourtClient",
                ["Jwt:ExpiresInMinutes"] = "60"
            })
            .Build();
        var service = new JwtTokenService(configuration);

        var result = service.GenerateAccessToken(new User
        {
            Id = 1,
            Email = "player@example.com",
            Role = UserRole.Player
        }, new UserProfile
        {
            FullName = "Nguyen Van A"
        });

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        Assert.Contains(token.Claims, claim => claim.Type == "role" && claim.Value == "Player");
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }
}
