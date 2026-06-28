using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.DTOs.Matches;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class MatchServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidPlayerAndSport_CreatesHostParticipant()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        await context.SaveChangesAsync();

        var response = await new MatchService(context).CreateAsync(
            host.UserId,
            CreateRequest(sport.Id, maxParticipants: 4));

        Assert.True(response.Success);
        Assert.True(response.Data!.IsHost);
        Assert.Equal(1, response.Data.ParticipantCount);
        Assert.Equal(3, response.Data.AvailableSlots);
        var participant = Assert.Single(context.MatchParticipants);
        Assert.True(participant.IsHost);
        Assert.Equal(host.Id, participant.PlayerId);
    }

    [Fact]
    public async Task CreateAsync_WhenSportIsNotInProfile_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport: null, SkillLevel.Beginner, "Da Nang");
        await context.SaveChangesAsync();

        var response = await new MatchService(context).CreateAsync(
            host.UserId,
            CreateRequest(sport.Id));

        Assert.False(response.Success);
        Assert.Contains("Add this sport", response.Message);
        Assert.Empty(context.Matches);
    }

    [Fact]
    public async Task GetRecommendedAsync_FiltersIncompatibleSkillAndPrioritizesSameCity()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var localHost = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var remoteHost = AddPlayer(context, sport, SkillLevel.Intermediate, "Ha Noi");
        var advancedHost = AddPlayer(context, sport, SkillLevel.Advanced, "Da Nang");
        AddMatch(context, localHost, sport, "Da Nang", SkillLevel.Beginner, SkillLevel.Advanced);
        AddMatch(context, remoteHost, sport, "Ha Noi", SkillLevel.Beginner, SkillLevel.Advanced);
        AddMatch(context, advancedHost, sport, "Da Nang", SkillLevel.Advanced, SkillLevel.Advanced);
        await context.SaveChangesAsync();

        var response = await new MatchService(context).GetRecommendedAsync(player.UserId);

        Assert.True(response.Success);
        Assert.Equal(2, response.Data!.Count);
        Assert.Equal("Da Nang", response.Data[0].LocationDescription);
        Assert.DoesNotContain(response.Data, item => item.HostProfileId == advancedHost.Id);
    }

    [Fact]
    public async Task RequestToJoinAsync_WhenSkillIsOutsideRange_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Advanced, "Da Nang");
        var beginner = AddPlayer(context, sport, SkillLevel.Beginner, "Da Nang");
        var match = AddMatch(
            context,
            host,
            sport,
            "Da Nang",
            SkillLevel.Intermediate,
            SkillLevel.Advanced);
        await context.SaveChangesAsync();

        var response = await new MatchService(context).RequestToJoinAsync(
            beginner.UserId,
            match.Id);

        Assert.False(response.Success);
        Assert.Contains("skill level", response.Message);
        Assert.Empty(context.MatchJoinRequests);
    }

    [Fact]
    public async Task RequestToJoinAsync_AfterRejection_ReusesExistingDatabaseRow()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        match.JoinRequests.Add(new MatchJoinRequest
        {
            Player = player,
            Status = MatchJoinRequestStatus.Rejected,
            RequestedAt = DateTimeOffset.UtcNow.AddHours(-1),
            RespondedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var response = await new MatchService(context).RequestToJoinAsync(player.UserId, match.Id);

        Assert.True(response.Success);
        Assert.Equal("Pending", response.Data!.Status);
        var storedRequest = Assert.Single(context.MatchJoinRequests);
        Assert.Null(storedRequest.RespondedAt);
    }

    [Fact]
    public async Task ApproveJoinRequest_WhenLastSlotIsFilled_AddsPlayerAndMarksMatchFull()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang", maxParticipants: 2);
        await context.SaveChangesAsync();
        var service = new MatchService(context);
        var joinResponse = await service.RequestToJoinAsync(player.UserId, match.Id);

        var response = await service.RespondToJoinRequestAsync(
            host.UserId,
            match.Id,
            joinResponse.Data!.Id,
            new RespondJoinRequestDto { Status = "Approved" });

        Assert.True(response.Success);
        Assert.Equal("Approved", response.Data!.Status);
        Assert.Equal(MatchStatus.Full, context.Matches.Single().Status);
        Assert.Contains(context.MatchParticipants, item => item.PlayerId == player.Id);
    }

    [Fact]
    public async Task GetCandidatesAsync_ExcludesParticipantsAndRanksSameCityFirst()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var localCandidate = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var remoteCandidate = AddPlayer(context, sport, SkillLevel.Intermediate, "Ha Noi");
        var participant = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        match.Participants.Add(new MatchParticipant
        {
            Player = participant,
            JoinedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var response = await new MatchService(context).GetCandidatesAsync(host.UserId, match.Id);

        Assert.True(response.Success);
        Assert.Equal(localCandidate.Id, response.Data![0].ProfileId);
        Assert.Contains(response.Data, item => item.ProfileId == remoteCandidate.Id);
        Assert.DoesNotContain(response.Data, item => item.ProfileId == participant.Id);
    }

    [Fact]
    public async Task RespondToInvitationAsync_WhenAccepted_AddsInviteeToMatch()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var invitee = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        await context.SaveChangesAsync();
        var service = new MatchService(context);
        var inviteResponse = await service.InviteAsync(
            host.UserId,
            match.Id,
            new CreateMatchInvitationDto { InviteeProfileId = invitee.Id });

        var response = await service.RespondToInvitationAsync(
            invitee.UserId,
            inviteResponse.Data!.Id,
            new RespondMatchInvitationDto { Status = "Accepted" });

        Assert.True(response.Success);
        Assert.Equal("Accepted", response.Data!.Status);
        Assert.Contains(context.MatchParticipants, item => item.PlayerId == invitee.Id);
    }

    [Fact]
    public async Task LeaveAsync_WhenMatchWasFull_RemovesPlayerAndReopensMatch()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang", maxParticipants: 2);
        match.Participants.Add(new MatchParticipant
        {
            Player = player,
            JoinedAt = DateTimeOffset.UtcNow
        });
        match.Status = MatchStatus.Full;
        await context.SaveChangesAsync();

        var response = await new MatchService(context).LeaveAsync(player.UserId, match.Id);

        Assert.True(response.Success);
        Assert.Equal("Open", response.Data!.Status);
        Assert.DoesNotContain(context.MatchParticipants, item => item.PlayerId == player.Id);
    }

    private static PlayCourtDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PlayCourtDbContext(options);
    }

    private static Sport AddSport(PlayCourtDbContext context)
    {
        var sport = new Sport
        {
            Code = $"SPORT-{Guid.NewGuid():N}",
            Name = "Badminton",
            IsActive = true
        };
        context.Sports.Add(sport);
        return sport;
    }

    private static UserProfile AddPlayer(
        PlayCourtDbContext context,
        Sport? sport,
        SkillLevel skillLevel,
        string city)
    {
        var profile = new UserProfile
        {
            User = new User
            {
                Email = $"{Guid.NewGuid():N}@example.com",
                PasswordHash = "hash",
                Role = UserRole.Player,
                Status = UserStatus.Active
            },
            FullName = $"Player {Guid.NewGuid():N}",
            City = city
        };
        if (sport is not null)
        {
            profile.PlayerSports.Add(new PlayerSport
            {
                Sport = sport,
                SkillLevel = skillLevel
            });
        }

        context.UserProfiles.Add(profile);
        return profile;
    }

    private static Match AddMatch(
        PlayCourtDbContext context,
        UserProfile host,
        Sport sport,
        string location,
        SkillLevel minimum = SkillLevel.Beginner,
        SkillLevel maximum = SkillLevel.Advanced,
        short maxParticipants = 4)
    {
        var match = new Match
        {
            Host = host,
            Sport = sport,
            LocationDescription = location,
            StartAt = DateTimeOffset.UtcNow.AddDays(1),
            EndAt = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            RequiredSkillLevelMin = minimum,
            RequiredSkillLevelMax = maximum,
            MaxParticipants = maxParticipants,
            Status = MatchStatus.Open
        };
        match.Participants.Add(new MatchParticipant
        {
            Player = host,
            IsHost = true,
            JoinedAt = DateTimeOffset.UtcNow
        });
        context.Matches.Add(match);
        return match;
    }

    private static CreateMatchRequestDto CreateRequest(int sportId, short maxParticipants = 4)
    {
        return new CreateMatchRequestDto
        {
            SportId = sportId,
            LocationDescription = "Da Nang",
            StartAt = DateTimeOffset.UtcNow.AddDays(1),
            EndAt = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            RequiredSkillLevelMin = 0,
            RequiredSkillLevelMax = 2,
            MaxParticipants = maxParticipants
        };
    }
}
