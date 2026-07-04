using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class VerificationTokenServiceTests
{
    [Fact]
    public async Task CreateOtpAsync_StoresNonBcryptHashAndVerifiesOnce()
    {
        await using var dbContext = CreateDbContext();
        var service = new VerificationTokenService(dbContext, CreateConfiguration());

        var otp = await service.CreateOtpAsync(1, VerificationTokenPurpose.EmailVerification);

        var token = await dbContext.VerificationTokens.SingleAsync();
        Assert.Matches(@"^\d{6}$", otp);
        Assert.False(token.TokenHash.StartsWith("$2", StringComparison.Ordinal));
        Assert.True(await service.VerifyOtpAsync(1, VerificationTokenPurpose.EmailVerification, otp));
        Assert.False(await service.VerifyOtpAsync(1, VerificationTokenPurpose.EmailVerification, otp));
    }

    private static PlayCourtDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PlayCourtDbContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "playcourt-development-secret-key-change-later-123456"
            })
            .Build();
    }
}
