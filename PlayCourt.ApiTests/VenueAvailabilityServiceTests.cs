using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class VenueAvailabilityServiceTests
{
    [Fact]
    public async Task GetAvailabilityAsync_ReturnsHalfHourSlotsWithAvailabilityStatusesAndPrices()
    {
        await using var context = CreateContext();
        var date = new DateOnly(2030, 7, 1);
        var venue = AddVenue(context);
        var court = AddCourt(context, venue, "Court 1");
        context.VenueOpeningHours.Add(new VenueOpeningHour
        {
            Venue = venue,
            DayOfWeek = 1,
            OpenTime = new TimeSpan(8, 0, 0),
            CloseTime = new TimeSpan(11, 0, 0)
        });
        context.PricingRules.Add(new PricingRule
        {
            Court = court,
            DayOfWeek = 1,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            PricePerHour = 100_000m,
            EffectiveFrom = date.ToDateTime(TimeOnly.MinValue)
        });
        context.Bookings.Add(new Booking
        {
            Court = court,
            UserProfile = AddPlayer(context),
            StartAt = At(date, 8, 30),
            EndAt = At(date, 9, 0),
            TotalPrice = 50_000m,
            PlatformFee = 2_500m,
            OwnerEarnings = 47_500m,
            Status = BookingStatus.Pending
        });
        context.CourtSchedules.Add(new CourtSchedule
        {
            Court = court,
            StartAt = At(date, 9, 0),
            EndAt = At(date, 9, 30),
            Reason = "Maintenance"
        });
        context.Bookings.Add(new Booking
        {
            Court = court,
            UserProfile = AddPlayer(context),
            StartAt = At(date, 10, 30),
            EndAt = At(date, 11, 0),
            TotalPrice = 50_000m,
            PlatformFee = 2_500m,
            OwnerEarnings = 47_500m,
            Status = BookingStatus.Confirmed
        });
        await context.SaveChangesAsync();

        var response = await new VenueService(context).GetAvailabilityAsync(venue.Id, date);

        Assert.True(response.Success, response.Message);
        var result = response.Data!;
        var availabilityCourt = result.Courts.Single();
        Assert.Equal(court.SportId, availabilityCourt.SportId);
        Assert.Equal(court.Sport.Name, availabilityCourt.SportName);
        Assert.Equal(48, availabilityCourt.Slots.Count);
        Assert.Equal("Closed", SlotAt(result, 7, 30).Status);
        Assert.Equal("Available", SlotAt(result, 8, 0).Status);
        Assert.False(SlotAt(result, 8, 0).CanStartBooking);
        Assert.Equal(50_000m, SlotAt(result, 8, 0).EstimatedPrice);
        Assert.Equal("Held", SlotAt(result, 8, 30).Status);
        Assert.False(SlotAt(result, 8, 30).CanStartBooking);
        Assert.Equal("Maintenance", SlotAt(result, 9, 0).Status);
        Assert.True(SlotAt(result, 9, 30).CanStartBooking);
        Assert.Equal("Booked", SlotAt(result, 10, 30).Status);
    }

    [Fact]
    public async Task GetAvailabilityAsync_PricesSlotCoveredByAdjacentRules()
    {
        await using var context = CreateContext();
        var date = new DateOnly(2030, 7, 1);
        var venue = AddVenue(context);
        var court = AddCourt(context, venue, "Court 1");
        context.VenueOpeningHours.Add(new VenueOpeningHour
        {
            Venue = venue,
            DayOfWeek = 1,
            OpenTime = new TimeSpan(8, 0, 0),
            CloseTime = new TimeSpan(9, 0, 0)
        });
        context.PricingRules.AddRange(
            new PricingRule
            {
                Court = court,
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(8, 15, 0),
                PricePerHour = 100_000m,
                EffectiveFrom = date.ToDateTime(TimeOnly.MinValue)
            },
            new PricingRule
            {
                Court = court,
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 15, 0),
                EndTime = new TimeSpan(8, 30, 0),
                PricePerHour = 200_000m,
                EffectiveFrom = date.ToDateTime(TimeOnly.MinValue)
            });
        await context.SaveChangesAsync();

        var response = await new VenueService(context).GetAvailabilityAsync(venue.Id, date);

        Assert.Equal(75_000m, SlotAt(response.Data!, 8, 0).EstimatedPrice);
    }

    [Fact]
    public async Task GetAvailabilityAsync_DoesNotAllowStartWhenNextSlotHasNoPrice()
    {
        await using var context = CreateContext();
        var date = new DateOnly(2030, 7, 1);
        var venue = AddVenue(context);
        var court = AddCourt(context, venue, "Court 1");
        context.VenueOpeningHours.Add(new VenueOpeningHour
        {
            Venue = venue,
            DayOfWeek = 1,
            OpenTime = new TimeSpan(8, 0, 0),
            CloseTime = new TimeSpan(9, 0, 0)
        });
        context.PricingRules.Add(new PricingRule
        {
            Court = court,
            DayOfWeek = 1,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(8, 30, 0),
            PricePerHour = 100_000m,
            EffectiveFrom = date.ToDateTime(TimeOnly.MinValue)
        });
        await context.SaveChangesAsync();

        var response = await new VenueService(context).GetAvailabilityAsync(venue.Id, date);

        Assert.False(SlotAt(response.Data!, 8, 0).CanStartBooking);
    }

    private static VenueAvailabilitySlotDto SlotAt(VenueAvailabilityResponseDto result, int hour, int minute) =>
        result.Courts.Single().Slots.Single(slot => slot.StartAt == At(result.Date, hour, minute));

    private static DateTimeOffset At(DateOnly date, int hour, int minute) =>
        new(date.Year, date.Month, date.Day, hour, minute, 0, TimeSpan.Zero);

    private static PlayCourtDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Venue AddVenue(PlayCourtDbContext context)
    {
        var owner = new UserProfile
        {
            User = new User { Email = $"{Guid.NewGuid():N}@example.com", PasswordHash = "hash", Role = UserRole.CourtOwner },
            FullName = "Owner"
        };
        var venue = new Venue
        {
            CourtOwnerProfile = new CourtOwnerProfile { UserProfile = owner, BusinessName = "Venue" },
            Name = "Test Venue",
            Address = "Da Nang",
            Status = VenueStatus.Approved
        };
        context.Venues.Add(venue);
        return venue;
    }

    private static Court AddCourt(PlayCourtDbContext context, Venue venue, string name)
    {
        var sport = new Sport { Code = $"S-{Guid.NewGuid():N}", Name = "Badminton" };
        var court = new Court { Venue = venue, Sport = sport, Name = name, Status = CourtStatus.Available };
        context.Courts.Add(court);
        return court;
    }

    private static UserProfile AddPlayer(PlayCourtDbContext context)
    {
        var player = new UserProfile
        {
            User = new User { Email = $"{Guid.NewGuid():N}@example.com", PasswordHash = "hash", Role = UserRole.Player },
            FullName = "Player"
        };
        context.UserProfiles.Add(player);
        return player;
    }
}
