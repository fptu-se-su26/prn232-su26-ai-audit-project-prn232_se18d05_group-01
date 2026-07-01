using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using PlayCourt.Application.Settings;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class BookingExpirationServiceTests
{
    [Fact]
    public async Task ExpirePendingBookingsAsync_WhenPendingBookingIsOlderThanTimeout_MarksBookingExpired()
    {
        await using var context = CreateContext();
        var now = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, now.AddMinutes(-20), BookingStatus.Pending);
        await context.SaveChangesAsync();
        var service = CreateService(context, timeoutMinutes: 15);

        var expiredCount = await service.ExpirePendingBookingsAsync(now);

        Assert.Equal(1, expiredCount);
        Assert.Equal(BookingStatus.Expired, booking.Status);
        Assert.NotNull(booking.UpdatedAt);
        var history = Assert.Single(context.BookingStatusHistories, item => item.BookingId == booking.Id);
        Assert.Equal(BookingStatus.Pending, history.OldStatus);
        Assert.Equal(BookingStatus.Expired, history.NewStatus);
        Assert.Null(history.ChangedByUserId);
        Assert.Equal("Booking expired because PayOS payment was not completed before timeout.", history.Reason);
    }

    [Fact]
    public async Task ExpirePendingBookingsAsync_WhenPendingBookingIsFresh_DoesNotExpire()
    {
        await using var context = CreateContext();
        var now = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, now.AddMinutes(-10), BookingStatus.Pending);
        await context.SaveChangesAsync();
        var service = CreateService(context, timeoutMinutes: 15);

        var expiredCount = await service.ExpirePendingBookingsAsync(now);

        Assert.Equal(0, expiredCount);
        Assert.Equal(BookingStatus.Pending, booking.Status);
        Assert.Empty(context.BookingStatusHistories);
    }

    [Fact]
    public async Task ExpirePendingBookingsAsync_WhenBookingWasConfirmed_DoesNotExpire()
    {
        await using var context = CreateContext();
        var now = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, now.AddMinutes(-20), BookingStatus.Confirmed);
        await context.SaveChangesAsync();
        var service = CreateService(context, timeoutMinutes: 15);

        var expiredCount = await service.ExpirePendingBookingsAsync(now);

        Assert.Equal(0, expiredCount);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        Assert.Empty(context.BookingStatusHistories);
    }

    [Fact]
    public async Task ExpirePendingBookingsAsync_WhenPendingPayOsPaymentExists_MarksPaymentFailed()
    {
        await using var context = CreateContext();
        var now = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, now.AddMinutes(-20), BookingStatus.Pending);
        var payment = new Payment
        {
            UserId = player.UserId,
            Booking = booking,
            Amount = booking.TotalPrice,
            Provider = "payOS",
            TransactionCode = "123456",
            Type = PaymentType.BookingPayment,
            Status = PaymentStatus.Pending,
            Currency = "VND",
            CreatedAt = now.AddMinutes(-19)
        };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();
        var service = CreateService(context, timeoutMinutes: 15);

        var expiredCount = await service.ExpirePendingBookingsAsync(now);

        Assert.Equal(1, expiredCount);
        Assert.Equal(BookingStatus.Expired, booking.Status);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Contains("expired", payment.Note, StringComparison.OrdinalIgnoreCase);
    }

    private static BookingExpirationService CreateService(PlayCourtDbContext context, int timeoutMinutes)
    {
        return new BookingExpirationService(
            context,
            Options.Create(new BookingExpirationSettings
            {
                PendingPaymentTimeoutMinutes = timeoutMinutes,
                ScanIntervalSeconds = 60,
                BatchSize = 100
            }));
    }

    private static PlayCourtDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new PlayCourtDbContext(options);
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

    private static Booking AddBooking(
        PlayCourtDbContext context,
        UserProfile player,
        DateTimeOffset createdAt,
        BookingStatus status)
    {
        var sport = new Sport
        {
            Code = $"SPORT-{Guid.NewGuid():N}",
            Name = "Badminton",
            IsActive = true
        };
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
        var venue = new Venue
        {
            CourtOwnerProfile = new CourtOwnerProfile
            {
                UserProfile = ownerProfile,
                BusinessName = "Test Venue Owner",
                VerificationStatus = CourtOwnerVerificationStatus.Approved
            },
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
        var booking = new Booking
        {
            UserProfile = player,
            Court = court,
            StartAt = createdAt.AddDays(1),
            EndAt = createdAt.AddDays(1).AddHours(2),
            TotalPrice = 120_000m,
            PlatformFee = 6_000m,
            OwnerEarnings = 114_000m,
            Status = status,
            CreatedAt = createdAt
        };
        context.Bookings.Add(booking);
        return booking;
    }
}
