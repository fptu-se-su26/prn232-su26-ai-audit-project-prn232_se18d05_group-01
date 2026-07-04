using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.DTOs.Users;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class UserServicePlayerSportTests
{
    [Fact]
    public async Task GetCurrentUserSportsAsync_WhenProfileHasSports_ReturnsOrderedSports()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        var football = AddSport(context, "FOOTBALL", "Football");
        var badminton = AddSport(context, "BADMINTON", "Badminton");
        context.PlayerSports.AddRange(
            new PlayerSport { UserProfile = profile, Sport = football, SkillLevel = SkillLevel.Advanced },
            new PlayerSport { UserProfile = profile, Sport = badminton, SkillLevel = SkillLevel.Intermediate });
        await context.SaveChangesAsync();

        var response = await new UserService(context).GetCurrentUserSportsAsync(profile.UserId);

        Assert.True(response.Success);
        Assert.Collection(
            response.Data!,
            item => Assert.Equal("Badminton", item.SportName),
            item => Assert.Equal("Football", item.SportName));
    }

    [Fact]
    public async Task GetCurrentUserSportsAsync_WhenProfileHasNoSports_ReturnsEmptyList()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        await context.SaveChangesAsync();

        var response = await new UserService(context).GetCurrentUserSportsAsync(profile.UserId);

        Assert.True(response.Success);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task GetCurrentUserSportsAsync_WhenProfileIsMissing_ReturnsFailure()
    {
        await using var context = CreateContext();

        var response = await new UserService(context).GetCurrentUserSportsAsync(404);

        Assert.False(response.Success);
        Assert.Equal("User profile not found.", response.Message);
    }

    [Fact]
    public async Task AddCurrentUserSportAsync_WhenRequestIsValid_AddsSport()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        var sport = AddSport(context, "BADMINTON", "Badminton");
        await context.SaveChangesAsync();

        var response = await new UserService(context).AddCurrentUserSportAsync(
            profile.UserId,
            new AddPlayerSportRequestDto { SportId = sport.Id, SkillLevel = 1 });

        Assert.True(response.Success);
        Assert.Equal("Intermediate", response.Data!.SkillLevel);
        Assert.Equal("BADMINTON", response.Data.SportCode);
        Assert.Single(context.PlayerSports);
    }

    [Theory]
    [InlineData(0, 1, "Sport not found.")]
    [InlineData(1, 99, "Skill level is invalid.")]
    public async Task AddCurrentUserSportAsync_WhenRequestIsInvalid_ReturnsFailure(
        int sportId,
        int skillLevel,
        string expectedMessage)
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        await context.SaveChangesAsync();

        var response = await new UserService(context).AddCurrentUserSportAsync(
            profile.UserId,
            new AddPlayerSportRequestDto { SportId = sportId, SkillLevel = skillLevel });

        Assert.False(response.Success);
        Assert.Equal(expectedMessage, response.Message);
    }

    [Fact]
    public async Task AddCurrentUserSportAsync_WhenSportIsInactive_ReturnsFailure()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        var sport = AddSport(context, "TENNIS", "Tennis", isActive: false);
        await context.SaveChangesAsync();

        var response = await new UserService(context).AddCurrentUserSportAsync(
            profile.UserId,
            new AddPlayerSportRequestDto { SportId = sport.Id, SkillLevel = 0 });

        Assert.False(response.Success);
        Assert.Equal("Sport is inactive.", response.Message);
    }

    [Fact]
    public async Task AddCurrentUserSportAsync_WhenSportAlreadyExists_ReturnsFailure()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        var sport = AddSport(context, "TENNIS", "Tennis");
        context.PlayerSports.Add(new PlayerSport
        {
            UserProfile = profile,
            Sport = sport,
            SkillLevel = SkillLevel.Beginner
        });
        await context.SaveChangesAsync();

        var response = await new UserService(context).AddCurrentUserSportAsync(
            profile.UserId,
            new AddPlayerSportRequestDto { SportId = sport.Id, SkillLevel = 2 });

        Assert.False(response.Success);
        Assert.Equal("Sport already exists in user profile.", response.Message);
    }

    [Fact]
    public async Task UpdateCurrentUserSportAsync_WhenSportExists_UpdatesSkillLevel()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        var sport = AddSport(context, "TENNIS", "Tennis");
        context.PlayerSports.Add(new PlayerSport
        {
            UserProfile = profile,
            Sport = sport,
            SkillLevel = SkillLevel.Beginner
        });
        await context.SaveChangesAsync();

        var response = await new UserService(context).UpdateCurrentUserSportAsync(
            profile.UserId,
            sport.Id,
            new UpdatePlayerSportRequestDto { SkillLevel = 2 });

        Assert.True(response.Success);
        Assert.Equal("Advanced", response.Data!.SkillLevel);
        Assert.Equal(SkillLevel.Advanced, context.PlayerSports.Single().SkillLevel);
    }

    [Fact]
    public async Task UpdateCurrentUserSportAsync_WhenSkillLevelIsInvalid_ReturnsFailure()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        await context.SaveChangesAsync();

        var response = await new UserService(context).UpdateCurrentUserSportAsync(
            profile.UserId,
            1,
            new UpdatePlayerSportRequestDto { SkillLevel = -1 });

        Assert.False(response.Success);
        Assert.Equal("Skill level is invalid.", response.Message);
    }

    [Fact]
    public async Task UpdateCurrentUserSportAsync_WhenSportIsNotInProfile_ReturnsFailure()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        var sport = AddSport(context, "TENNIS", "Tennis");
        await context.SaveChangesAsync();

        var response = await new UserService(context).UpdateCurrentUserSportAsync(
            profile.UserId,
            sport.Id,
            new UpdatePlayerSportRequestDto { SkillLevel = 1 });

        Assert.False(response.Success);
        Assert.Equal("Sport is not in user profile.", response.Message);
    }

    [Fact]
    public async Task RemoveCurrentUserSportAsync_WhenSportExists_RemovesAndReturnsSport()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        var sport = AddSport(context, "TENNIS", "Tennis");
        context.PlayerSports.Add(new PlayerSport
        {
            UserProfile = profile,
            Sport = sport,
            SkillLevel = SkillLevel.Intermediate
        });
        await context.SaveChangesAsync();

        var response = await new UserService(context).RemoveCurrentUserSportAsync(profile.UserId, sport.Id);

        Assert.True(response.Success);
        Assert.Equal("TENNIS", response.Data!.SportCode);
        Assert.Empty(context.PlayerSports);
    }

    [Fact]
    public async Task RemoveCurrentUserSportAsync_WhenSportIsNotInProfile_ReturnsFailure()
    {
        await using var context = CreateContext();
        var profile = AddUserProfile(context);
        await context.SaveChangesAsync();

        var response = await new UserService(context).RemoveCurrentUserSportAsync(profile.UserId, 99);

        Assert.False(response.Success);
        Assert.Equal("Sport is not in user profile.", response.Message);
    }

    private static PlayCourtDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PlayCourtDbContext(options);
    }

    private static UserProfile AddUserProfile(PlayCourtDbContext context)
    {
        var user = new User
        {
            Email = $"{Guid.NewGuid():N}@example.com",
            PasswordHash = "hash",
            Role = UserRole.Player,
            Status = UserStatus.Active
        };
        var profile = new UserProfile
        {
            User = user,
            FullName = "Test Player"
        };

        context.UserProfiles.Add(profile);
        return profile;
    }

    private static Sport AddSport(
        PlayCourtDbContext context,
        string code,
        string name,
        bool isActive = true)
    {
        var sport = new Sport
        {
            Code = code,
            Name = name,
            IsActive = isActive
        };

        context.Sports.Add(sport);
        return sport;
    }
}
