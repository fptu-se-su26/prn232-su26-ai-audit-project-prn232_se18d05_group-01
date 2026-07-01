using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PlayCourt.Application.DTOs.Bookings;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class BookingServiceTests
{
    [Theory]
    [InlineData(MatchStatus.Open)]
    [InlineData(MatchStatus.Full)]
    public async Task CreateAsync_WhenActiveMatchOverlaps_RejectsBooking(MatchStatus matchStatus)
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var player = AddPlayer(context);
        var host = AddPlayer(context);
        var court = AddAvailableCourt(context, sport);
        var startAt = new DateTimeOffset(2030, 7, 1, 16, 0, 0, TimeSpan.Zero);
        var endAt = startAt.AddHours(2);
        AddPricingRule(context, court, startAt, new TimeSpan(14, 0, 0), new TimeSpan(20, 0, 0), 100_000m);
        AddMatch(context, host, sport, court, startAt, endAt, matchStatus);
        await context.SaveChangesAsync();

        var response = await CreateService(context).CreateAsync(
            player.UserId,
            new CreateBookingRequestDto
            {
                CourtId = court.Id,
                StartAt = startAt,
                EndAt = endAt
            });

        Assert.False(response.Success);
        Assert.Equal("Court already has an active match in this time range.", response.Message);
        Assert.Empty(context.Bookings.Where(item => item.UserProfileId == player.Id));
    }

    [Fact]
    public async Task CreateAsync_WhenPricingRulesCoverAdjacentTimeRanges_AddsSegmentPrices()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var player = AddPlayer(context);
        var court = AddAvailableCourt(context, sport);
        var startAt = new DateTimeOffset(2030, 7, 1, 16, 0, 0, TimeSpan.Zero);
        var endAt = startAt.AddHours(2);
        AddPricingRule(context, court, startAt, new TimeSpan(14, 0, 0), new TimeSpan(17, 0, 0), 100_000m);
        AddPricingRule(context, court, startAt, new TimeSpan(17, 0, 0), new TimeSpan(20, 0, 0), 150_000m);
        await context.SaveChangesAsync();

        var response = await CreateService(context).CreateAsync(
            player.UserId,
            new CreateBookingRequestDto
            {
                CourtId = court.Id,
                StartAt = startAt,
                EndAt = endAt
            });

        Assert.True(response.Success);
        Assert.Equal(250_000m, response.Data!.TotalPrice);
        Assert.Equal(12_500m, response.Data.PlatformFee);
        Assert.Equal(237_500m, response.Data.OwnerEarnings);
    }

    [Fact]
    public async Task CreateAsync_WhenExpiredBookingOverlaps_CreatesBooking()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var player = AddPlayer(context);
        var existingPlayer = AddPlayer(context);
        var court = AddAvailableCourt(context, sport);
        var startAt = new DateTimeOffset(2030, 7, 1, 16, 0, 0, TimeSpan.Zero);
        var endAt = startAt.AddHours(2);
        AddPricingRule(context, court, startAt, new TimeSpan(14, 0, 0), new TimeSpan(20, 0, 0), 100_000m);
        context.Bookings.Add(new Booking
        {
            UserProfile = existingPlayer,
            Court = court,
            StartAt = startAt,
            EndAt = endAt,
            TotalPrice = 200_000m,
            PlatformFee = 10_000m,
            OwnerEarnings = 190_000m,
            Status = BookingStatus.Expired,
            CreatedAt = startAt.AddDays(-1)
        });
        await context.SaveChangesAsync();

        var response = await CreateService(context).CreateAsync(
            player.UserId,
            new CreateBookingRequestDto
            {
                CourtId = court.Id,
                StartAt = startAt,
                EndAt = endAt
            });

        Assert.True(response.Success);
        Assert.Equal(2, context.Bookings.Count());
        Assert.Equal(BookingStatus.Pending, context.Bookings.Single(item => item.UserProfileId == player.Id).Status);
    }

    [Fact]
    public async Task CreateAsync_WhenActiveBookingOverlaps_RejectsBooking()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var player = AddPlayer(context);
        var existingPlayer = AddPlayer(context);
        var court = AddAvailableCourt(context, sport);
        var startAt = new DateTimeOffset(2030, 7, 1, 16, 0, 0, TimeSpan.Zero);
        var endAt = startAt.AddHours(2);
        AddPricingRule(context, court, startAt, new TimeSpan(14, 0, 0), new TimeSpan(20, 0, 0), 100_000m);
        context.Bookings.Add(new Booking
        {
            UserProfile = existingPlayer,
            Court = court,
            StartAt = startAt.AddMinutes(30),
            EndAt = endAt.AddMinutes(30),
            TotalPrice = 200_000m,
            PlatformFee = 10_000m,
            OwnerEarnings = 190_000m,
            Status = BookingStatus.Pending,
            CreatedAt = startAt.AddDays(-1)
        });
        await context.SaveChangesAsync();

        var response = await CreateService(context).CreateAsync(
            player.UserId,
            new CreateBookingRequestDto
            {
                CourtId = court.Id,
                StartAt = startAt,
                EndAt = endAt
            });

        Assert.False(response.Success);
        Assert.Equal("Court already has an active booking in this time range.", response.Message);
        Assert.Single(context.Bookings.Where(item => item.CourtId == court.Id));
    }

    [Fact]
    public async Task CreateAsync_WhenVenueIsClosedOnBookingDay_RejectsBooking()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var player = AddPlayer(context);
        var court = AddAvailableCourt(context, sport);
        var startAt = new DateTimeOffset(2030, 7, 1, 16, 0, 0, TimeSpan.Zero);
        var endAt = startAt.AddHours(2);
        AddPricingRule(context, court, startAt, new TimeSpan(14, 0, 0), new TimeSpan(20, 0, 0), 100_000m);
        AddOpeningHour(context, court, startAt, null, null, isClosed: true);
        await context.SaveChangesAsync();

        var response = await CreateService(context).CreateAsync(
            player.UserId,
            new CreateBookingRequestDto
            {
                CourtId = court.Id,
                StartAt = startAt,
                EndAt = endAt
            });

        Assert.False(response.Success);
        Assert.Equal("Venue is closed on this day.", response.Message);
        Assert.Empty(context.Bookings);
    }

    [Fact]
    public async Task CreateAsync_WhenBookingIsOutsideVenueOpeningHours_RejectsBooking()
    {
        await using var context = CreateContext();
        var sport = AddSport(context);
        var player = AddPlayer(context);
        var court = AddAvailableCourt(context, sport);
        var startAt = new DateTimeOffset(2030, 7, 1, 6, 0, 0, TimeSpan.Zero);
        var endAt = startAt.AddHours(1);
        AddPricingRule(context, court, startAt, new TimeSpan(0, 0, 0), new TimeSpan(23, 0, 0), 100_000m);
        AddOpeningHour(context, court, startAt, new TimeSpan(8, 0, 0), new TimeSpan(22, 0, 0));
        await context.SaveChangesAsync();

        var response = await CreateService(context).CreateAsync(
            player.UserId,
            new CreateBookingRequestDto
            {
                CourtId = court.Id,
                StartAt = startAt,
                EndAt = endAt
            });

        Assert.False(response.Success);
        Assert.Equal("Booking time is outside venue opening hours.", response.Message);
        Assert.Empty(context.Bookings);
    }

    private static PlayCourtDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new PlayCourtDbContext(options);
    }

    private static BookingService CreateService(PlayCourtDbContext context)
    {
        return new BookingService(context, new NotificationWriter(context));
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
            FullName = $"Player {Guid.NewGuid():N}"
        };
        context.UserProfiles.Add(profile);
        return profile;
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

    private static void AddPricingRule(
        PlayCourtDbContext context,
        Court court,
        DateTimeOffset bookingDate,
        TimeSpan startTime,
        TimeSpan endTime,
        decimal pricePerHour)
    {
        context.PricingRules.Add(new PricingRule
        {
            Court = court,
            DayOfWeek = bookingDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)bookingDate.DayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            PricePerHour = pricePerHour,
            EffectiveFrom = bookingDate.Date
        });
    }

    private static void AddOpeningHour(
        PlayCourtDbContext context,
        Court court,
        DateTimeOffset bookingDate,
        TimeSpan? openTime,
        TimeSpan? closeTime,
        bool isClosed = false)
    {
        context.VenueOpeningHours.Add(new VenueOpeningHour
        {
            Venue = court.Venue,
            DayOfWeek = bookingDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)bookingDate.DayOfWeek,
            OpenTime = openTime,
            CloseTime = closeTime,
            IsClosed = isClosed
        });
    }

    private static void AddMatch(
        PlayCourtDbContext context,
        UserProfile host,
        Sport sport,
        Court court,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        MatchStatus status = MatchStatus.Open)
    {
        var match = new Match
        {
            Host = host,
            Sport = sport,
            Court = court,
            StartAt = startAt,
            EndAt = endAt,
            MaxParticipants = 4,
            Status = status
        };
        match.Participants.Add(new MatchParticipant
        {
            Player = host,
            IsHost = true,
            JoinedAt = DateTimeOffset.UtcNow
        });
        context.Matches.Add(match);
    }
}
