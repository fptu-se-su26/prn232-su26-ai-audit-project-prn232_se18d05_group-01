using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using PlayCourt.Application.DTOs.Payments;
using PlayCourt.Application.Interfaces;
using PlayCourt.Application.Settings;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class PaymentServiceTests
{
    [Fact]
    public async Task CreatePayOsPaymentLinkAsync_WithPendingOwnedBooking_CreatesPendingPayment()
    {
        await using var context = CreateContext();
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, totalPrice: 120_000m);
        await context.SaveChangesAsync();
        var gateway = new FakePayOsGateway
        {
            CreateResult = new PayOsCreatePaymentLinkResult(
                "https://pay.payos.vn/web/abc",
                "link_123")
        };
        var service = CreateService(context, gateway);

        var response = await service.CreatePayOsPaymentLinkAsync(player.UserId, booking.Id);

        Assert.True(response.Success);
        Assert.Equal(booking.Id, response.Data!.BookingId);
        Assert.Equal("https://pay.payos.vn/web/abc", response.Data.CheckoutUrl);
        Assert.Equal("Pending", response.Data.Status);
        Assert.Equal(120_000, gateway.CreateRequest!.Amount);
        Assert.Contains($"bookingId={booking.Id}", gateway.CreateRequest.ReturnUrl);
        var payment = Assert.Single(context.Payments);
        Assert.Equal(player.UserId, payment.UserId);
        Assert.Equal(booking.Id, payment.BookingId);
        Assert.Equal("payOS", payment.Provider);
        Assert.Equal(PaymentType.BookingPayment, payment.Type);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(response.Data.OrderCode.ToString(), payment.TransactionCode);
    }

    [Fact]
    public async Task CreatePayOsPaymentLinkAsync_WhenBookingBelongsToAnotherPlayer_ReturnsFailure()
    {
        await using var context = CreateContext();
        var owner = AddPlayer(context);
        var stranger = AddPlayer(context);
        var booking = AddBooking(context, owner, totalPrice: 120_000m);
        await context.SaveChangesAsync();
        var gateway = new FakePayOsGateway();
        var service = CreateService(context, gateway);

        var response = await service.CreatePayOsPaymentLinkAsync(stranger.UserId, booking.Id);

        Assert.False(response.Success);
        Assert.Contains("authorized", response.Message);
        Assert.Empty(context.Payments);
        Assert.Null(gateway.CreateRequest);
    }

    [Fact]
    public async Task SyncPayOsPaymentAsync_WhenPayOsReportsPaid_MarksPaymentSuccess()
    {
        await using var context = CreateContext();
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, totalPrice: 120_000m);
        var payment = AddPendingPayOsPayment(context, player.UserId, booking, orderCode: 987654321);
        await context.SaveChangesAsync();
        var gateway = new FakePayOsGateway
        {
            StatusResult = new PayOsPaymentLinkStatusResult(
                987654321,
                "PAID",
                "TF230204212323",
                "link_123",
                120_000,
                """{"status":"PAID"}""")
        };
        var service = CreateService(context, gateway);

        var response = await service.SyncPayOsPaymentAsync(player.UserId, booking.Id);

        Assert.True(response.Success);
        Assert.Equal("Success", response.Data!.Status);
        Assert.Equal(987654321, gateway.StatusOrderCode);
        Assert.Equal(PaymentStatus.Success, payment.Status);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        var history = Assert.Single(context.BookingStatusHistories, item =>
            item.BookingId == booking.Id &&
            item.OldStatus == BookingStatus.Pending &&
            item.NewStatus == BookingStatus.Confirmed);
        Assert.Equal(player.UserId, history.ChangedByUserId);
        Assert.Equal("987654321", payment.TransactionCode);
        Assert.Contains("TF230204212323", payment.Note);
        Assert.NotNull(payment.PaidAt);
        Assert.Equal("""{"status":"PAID"}""", payment.ProviderPayload);
    }

    [Fact]
    public async Task SyncPayOsPaymentAsync_WhenPaymentAlreadySucceeded_DoesNotCallGatewayAgain()
    {
        await using var context = CreateContext();
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, totalPrice: 120_000m);
        var payment = AddPendingPayOsPayment(context, player.UserId, booking, orderCode: 987654321);
        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTimeOffset.UtcNow.AddMinutes(-5);
        await context.SaveChangesAsync();
        var gateway = new FakePayOsGateway();
        var service = CreateService(context, gateway);

        var response = await service.SyncPayOsPaymentAsync(player.UserId, booking.Id);

        Assert.True(response.Success);
        Assert.Equal("Success", response.Data!.Status);
        Assert.Null(gateway.StatusOrderCode);
    }

    [Theory]
    [InlineData("EXPIRED")]
    [InlineData("CANCELLED")]
    [InlineData("FAILED")]
    public async Task SyncPayOsPaymentAsync_WhenPayOsReportsTerminalFailure_MarksPaymentFailedAndLeavesBookingPending(
        string payOsStatus)
    {
        await using var context = CreateContext();
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, totalPrice: 120_000m);
        var payment = AddPendingPayOsPayment(context, player.UserId, booking, orderCode: 987654321);
        await context.SaveChangesAsync();
        var gateway = new FakePayOsGateway
        {
            StatusResult = new PayOsPaymentLinkStatusResult(
                987654321,
                payOsStatus,
                "TF230204212323",
                "link_123",
                120_000,
                $$"""{"status":"{{payOsStatus}}"}""")
        };
        var service = CreateService(context, gateway);

        var response = await service.SyncPayOsPaymentAsync(player.UserId, booking.Id);

        Assert.True(response.Success);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(BookingStatus.Pending, booking.Status);
        Assert.DoesNotContain(context.BookingStatusHistories, item =>
            item.BookingId == booking.Id &&
            item.NewStatus == BookingStatus.Confirmed);
    }

    [Fact]
    public async Task GetBookingPaymentsAsync_WhenPlayerOwnsBooking_ReturnsPaymentHistory()
    {
        await using var context = CreateContext();
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, totalPrice: 120_000m);
        AddPendingPayOsPayment(context, player.UserId, booking, orderCode: 987654321);
        await context.SaveChangesAsync();
        var service = CreateService(context, new FakePayOsGateway());

        var response = await service.GetBookingPaymentsAsync(player.UserId, booking.Id);

        Assert.True(response.Success);
        var payment = Assert.Single(response.Data!);
        Assert.Equal(booking.Id, payment.BookingId);
        Assert.Equal("payOS", payment.Provider);
        Assert.Equal("Pending", payment.Status);
    }

    [Fact]
    public async Task HandlePayOsWebhookAsync_WithVerifiedSuccessWebhook_MarksPaymentSuccess()
    {
        await using var context = CreateContext();
        var player = AddPlayer(context);
        var booking = AddBooking(context, player, totalPrice: 120_000m);
        var payment = AddPendingPayOsPayment(context, player.UserId, booking, orderCode: 987654321);
        await context.SaveChangesAsync();
        var gateway = new FakePayOsGateway
        {
            WebhookResult = new PayOsWebhookResult(
                987654321,
                120_000,
                "TF230204212323",
                "link_123",
                true,
                """{"success":true}""")
        };
        var service = CreateService(context, gateway);

        var response = await service.HandlePayOsWebhookAsync(JsonDocument.Parse("""{"success":true}""").RootElement);

        Assert.True(response.Success);
        Assert.Equal("Success", response.Data!.Status);
        Assert.Equal(PaymentStatus.Success, payment.Status);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        Assert.Contains(context.BookingStatusHistories, item =>
            item.BookingId == booking.Id &&
            item.OldStatus == BookingStatus.Pending &&
            item.NewStatus == BookingStatus.Confirmed &&
            item.ChangedByUserId == player.UserId);
        Assert.NotNull(payment.PaidAt);
        Assert.Equal("""{"success":true}""", payment.ProviderPayload);
    }

    private static PaymentService CreateService(PlayCourtDbContext context, IPayOsGateway gateway)
    {
        return new PaymentService(
            context,
            gateway,
            Options.Create(new PayOsSettings
            {
                ClientId = "test-client-id",
                ApiKey = "test-api-key",
                ChecksumKey = "test-checksum-key",
                ReturnUrl = "http://localhost:5173/payment/result",
                CancelUrl = "http://localhost:5173/payment/cancel"
            }),
            new NotificationWriter(context));
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

    private static Booking AddBooking(PlayCourtDbContext context, UserProfile player, decimal totalPrice)
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
        var platformFee = Math.Round(totalPrice * 0.05m, 2);
        var booking = new Booking
        {
            UserProfile = player,
            Court = court,
            StartAt = DateTimeOffset.UtcNow.AddDays(1),
            EndAt = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            TotalPrice = totalPrice,
            PlatformFee = platformFee,
            OwnerEarnings = totalPrice - platformFee,
            Status = BookingStatus.Pending
        };
        context.Bookings.Add(booking);
        return booking;
    }

    private static Payment AddPendingPayOsPayment(
        PlayCourtDbContext context,
        int userId,
        Booking booking,
        long orderCode)
    {
        var payment = new Payment
        {
            UserId = userId,
            Booking = booking,
            Amount = booking.TotalPrice,
            Provider = "payOS",
            TransactionCode = orderCode.ToString(),
            Type = PaymentType.BookingPayment,
            Status = PaymentStatus.Pending,
            Currency = "VND"
        };
        context.Payments.Add(payment);
        return payment;
    }

    private sealed class FakePayOsGateway : IPayOsGateway
    {
        public PayOsCreatePaymentLinkRequest? CreateRequest { get; private set; }
        public PayOsCreatePaymentLinkResult CreateResult { get; set; } =
            new("https://pay.payos.vn/web/default", "link_default");
        public long? StatusOrderCode { get; private set; }
        public PayOsPaymentLinkStatusResult StatusResult { get; set; } =
            new(1, "PENDING", null, null, 0, "{}");
        public PayOsWebhookResult WebhookResult { get; set; } =
            new(1, 0, null, null, true, "{}");

        public Task<PayOsCreatePaymentLinkResult> CreatePaymentLinkAsync(
            PayOsCreatePaymentLinkRequest request,
            CancellationToken cancellationToken = default)
        {
            CreateRequest = request;
            return Task.FromResult(CreateResult);
        }

        public Task<PayOsPaymentLinkStatusResult> GetPaymentLinkInformationAsync(
            long orderCode,
            CancellationToken cancellationToken = default)
        {
            StatusOrderCode = orderCode;
            return Task.FromResult(StatusResult);
        }

        public Task<PayOsWebhookResult> VerifyWebhookAsync(
            string webhookBody,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(WebhookResult);
        }
    }
}
