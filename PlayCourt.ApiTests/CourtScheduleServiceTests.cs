using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.DTOs.CourtSchedules;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class CourtScheduleServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenOverlapsActiveBooking_ReturnsFailure()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var owner = AddCourtOwner(context);
        var player = AddPlayer(context);
        var court = AddAvailableCourt(context, owner, sport);
        var startAt = DateTimeOffset.UtcNow.AddDays(1);
        context.Bookings.Add(new Booking
        {
            UserProfile = player,
            Court = court,
            StartAt = startAt,
            EndAt = startAt.AddHours(2),
            TotalPrice = 100_000m,
            PlatformFee = 5_000m,
            OwnerEarnings = 95_000m,
            Status = BookingStatus.Confirmed
        });
        await context.SaveChangesAsync();

        var response = await CreateService(context).CreateAsync(
            court.Id,
            owner.UserId,
            new CreateCourtScheduleRequestDto
            {
                StartAt = startAt.AddMinutes(30),
                EndAt = startAt.AddHours(1),
                Reason = "Maintenance"
            });

        Assert.False(response.Success);
        Assert.Contains(response.Errors, error => error.Contains("đơn đặt sân"));
        Assert.Empty(context.CourtSchedules);
    }

    private static PlayCourtDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PlayCourtDbContext(options);
    }

    private static CourtScheduleService CreateService(PlayCourtDbContext context)
    {
        return new CourtScheduleService(context);
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

    private static UserProfile AddCourtOwner(PlayCourtDbContext context)
    {
        var profile = new UserProfile
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
        context.UserProfiles.Add(profile);
        return profile;
    }

    private static UserProfile AddPlayer(PlayCourtDbContext context)
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
            FullName = "Player"
        };
        context.UserProfiles.Add(profile);
        return profile;
    }

    private static Court AddAvailableCourt(PlayCourtDbContext context, UserProfile owner, Sport sport)
    {
        var courtOwner = new CourtOwnerProfile
        {
            UserProfile = owner,
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
}
