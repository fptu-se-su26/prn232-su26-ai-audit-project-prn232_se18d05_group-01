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

    [Fact]
    public async Task CreateAsync_WhenCourtOverlapsActiveBooking_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var court = AddAvailableCourt(context, sport);
        await context.SaveChangesAsync();
        var request = CreateRequest(sport.Id);
        request.CourtId = court.Id;
        request.LocationDescription = null;
        context.Bookings.Add(new Booking
        {
            UserProfileId = host.Id,
            CourtId = court.Id,
            StartAt = request.StartAt.AddMinutes(30),
            EndAt = request.EndAt.AddMinutes(30),
            TotalPrice = 100_000,
            PlatformFee = 10_000,
            OwnerEarnings = 90_000,
            Status = BookingStatus.Confirmed
        });
        await context.SaveChangesAsync();

        var response = await new MatchService(context).CreateAsync(host.UserId, request);

        Assert.False(response.Success);
        Assert.Contains("active booking", response.Message);
        Assert.Empty(context.Matches);
    }

    [Fact]
    public async Task CreateAsync_WhenCourtOverlapsBlockedSchedule_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var court = AddAvailableCourt(context, sport);
        await context.SaveChangesAsync();
        var request = CreateRequest(sport.Id);
        request.CourtId = court.Id;
        request.LocationDescription = null;
        context.CourtSchedules.Add(new CourtSchedule
        {
            CourtId = court.Id,
            StartAt = request.StartAt.AddMinutes(30),
            EndAt = request.EndAt.AddMinutes(30),
            Reason = "Maintenance"
        });
        await context.SaveChangesAsync();

        var response = await new MatchService(context).CreateAsync(host.UserId, request);

        Assert.False(response.Success);
        Assert.Contains("blocked", response.Message);
        Assert.Empty(context.Matches);
    }

    [Fact]
    public async Task CreateAsync_WhenCourtOverlapsAnotherActiveMatch_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var otherHost = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var court = AddAvailableCourt(context, sport);
        var existingMatch = AddMatch(context, otherHost, sport, "Da Nang");
        existingMatch.Court = court;
        await context.SaveChangesAsync();
        var request = CreateRequest(sport.Id);
        request.CourtId = court.Id;
        request.LocationDescription = null;

        var response = await new MatchService(context).CreateAsync(host.UserId, request);

        Assert.False(response.Success);
        Assert.Contains("another match", response.Message);
        Assert.Single(context.Matches);
    }

    [Fact]
    public async Task UpdateAsync_WhenCourtAndTimeAreUnchanged_DoesNotConflictWithItself()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var court = AddAvailableCourt(context, sport);
        var match = AddMatch(context, host, sport, "Da Nang");
        match.Court = court;
        await context.SaveChangesAsync();
        var request = new UpdateMatchRequestDto
        {
            SportId = sport.Id,
            CourtId = court.Id,
            StartAt = match.StartAt,
            EndAt = match.EndAt,
            RequiredSkillLevelMin = 0,
            RequiredSkillLevelMax = 2,
            MaxParticipants = match.MaxParticipants,
            Description = "Updated"
        };

        var response = await new MatchService(context).UpdateAsync(host.UserId, match.Id, request);

        Assert.True(response.Success);
        Assert.Equal("Updated", response.Data!.Description);
    }

    [Fact]
    public async Task ApproveJoinRequest_WhenPlayerBecameInactive_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        await context.SaveChangesAsync();
        var service = new MatchService(context);
        var joinResponse = await service.RequestToJoinAsync(player.UserId, match.Id);
        player.User.Status = UserStatus.Inactive;
        await context.SaveChangesAsync();

        var response = await service.RespondToJoinRequestAsync(
            host.UserId,
            match.Id,
            joinResponse.Data!.Id,
            new RespondJoinRequestDto { Status = "Approved" });

        Assert.False(response.Success);
        Assert.Contains("no longer active", response.Message);
        Assert.DoesNotContain(context.MatchParticipants, item => item.PlayerId == player.Id);
    }

    [Fact]
    public async Task ApproveJoinRequest_WhenPlayerRemovedSport_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        await context.SaveChangesAsync();
        var service = new MatchService(context);
        var joinResponse = await service.RequestToJoinAsync(player.UserId, match.Id);
        context.PlayerSports.Remove(player.PlayerSports.Single());
        await context.SaveChangesAsync();

        var response = await service.RespondToJoinRequestAsync(
            host.UserId,
            match.Id,
            joinResponse.Data!.Id,
            new RespondJoinRequestDto { Status = "Approved" });

        Assert.False(response.Success);
        Assert.Contains("no longer has this sport", response.Message);
        Assert.DoesNotContain(context.MatchParticipants, item => item.PlayerId == player.Id);
    }

    [Fact]
    public async Task ApproveJoinRequest_WhenPlayerSkillChangedOutsideRange_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Advanced, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(
            context,
            host,
            sport,
            "Da Nang",
            SkillLevel.Intermediate,
            SkillLevel.Advanced);
        await context.SaveChangesAsync();
        var service = new MatchService(context);
        var joinResponse = await service.RequestToJoinAsync(player.UserId, match.Id);
        player.PlayerSports.Single().SkillLevel = SkillLevel.Beginner;
        await context.SaveChangesAsync();

        var response = await service.RespondToJoinRequestAsync(
            host.UserId,
            match.Id,
            joinResponse.Data!.Id,
            new RespondJoinRequestDto { Status = "Approved" });

        Assert.False(response.Success);
        Assert.Contains("skill level no longer meets", response.Message);
        Assert.DoesNotContain(context.MatchParticipants, item => item.PlayerId == player.Id);
    }

    [Theory]
    [InlineData(MatchStatus.Cancelled)]
    [InlineData(MatchStatus.Completed)]
    public async Task LeaveAsync_WhenMatchIsClosed_ReturnsFailure(MatchStatus status)
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        match.Participants.Add(new MatchParticipant
        {
            Player = player,
            JoinedAt = DateTimeOffset.UtcNow
        });
        match.Status = status;
        await context.SaveChangesAsync();

        var response = await new MatchService(context).LeaveAsync(player.UserId, match.Id);

        Assert.False(response.Success);
        Assert.Contains("cannot leave", response.Message);
        Assert.Contains(context.MatchParticipants, item => item.PlayerId == player.Id);
    }

    [Fact]
    public async Task LeaveAsync_AfterMatchStarted_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var player = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        match.StartAt = DateTimeOffset.UtcNow.AddHours(-1);
        match.EndAt = DateTimeOffset.UtcNow.AddHours(1);
        match.Participants.Add(new MatchParticipant
        {
            Player = player,
            JoinedAt = DateTimeOffset.UtcNow.AddDays(-1)
        });
        await context.SaveChangesAsync();

        var response = await new MatchService(context).LeaveAsync(player.UserId, match.Id);

        Assert.False(response.Success);
        Assert.Contains("after the match has started", response.Message);
        Assert.Contains(context.MatchParticipants, item => item.PlayerId == player.Id);
    }

    [Fact]
    public async Task CancelAsync_AfterMatchStarted_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var host = AddPlayer(context, sport, SkillLevel.Intermediate, "Da Nang");
        var match = AddMatch(context, host, sport, "Da Nang");
        match.StartAt = DateTimeOffset.UtcNow.AddHours(-1);
        match.EndAt = DateTimeOffset.UtcNow.AddHours(1);
        await context.SaveChangesAsync();

        var response = await new MatchService(context).CancelAsync(host.UserId, match.Id);

        Assert.False(response.Success);
        Assert.Contains("after it has started", response.Message);
        Assert.Equal(MatchStatus.Open, match.Status);
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

    private static Court AddAvailableCourt(PlayCourtDbContext context, Sport sport)
    {
        var ownerProfile = new UserProfile
        {
            User = new User
            {
                Email = $"{Guid.NewGuid():N}@example.com",
                PasswordHash = "hash",
                Role = UserRole.CourtOwner,
                Status = UserStatus.Active
            },
            FullName = "Court Owner"
        };
        var courtOwner = new CourtOwnerProfile
        {
            UserProfile = ownerProfile,
            BusinessName = "Test Venue Owner",
            VerificationStatus = CourtOwnerVerificationStatus.Approved
        };
        var venue = new Venue
        {
            CourtOwnerProfile = courtOwner,
            Name = "Test Venue",
            Address = "Da Nang",
            Status = VenueStatus.Approved
        };
        var court = new Court
        {
            Venue = venue,
            Sport = sport,
            Name = "Court 1",
            Status = CourtStatus.Available
        };
        context.Courts.Add(court);
        return court;
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
