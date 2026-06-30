using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Notifications;
using PlayCourt.Application.DTOs.Payments;
using PlayCourt.Application.Interfaces;
using PlayCourt.Application.Settings;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class PaymentService : IPaymentService
    {
        private const string PayOsProvider = "payOS";
        private readonly PlayCourtDbContext _dbContext;
        private readonly IPayOsGateway _payOsGateway;
        private readonly PayOsSettings _settings;
        private readonly INotificationWriter _notificationWriter;

        public PaymentService(
            PlayCourtDbContext dbContext,
            IPayOsGateway payOsGateway,
            IOptions<PayOsSettings> settings,
            INotificationWriter notificationWriter)
        {
            _dbContext = dbContext;
            _payOsGateway = payOsGateway;
            _settings = settings.Value;
            _notificationWriter = notificationWriter;
        }

        public async Task<ApiResponse<CreatePayOsPaymentResponseDto>> CreatePayOsPaymentLinkAsync(
            int userId,
            int bookingId)
        {
            if (!IsConfigured())
            {
                return ApiResponse<CreatePayOsPaymentResponseDto>.Fail("PayOS is not configured.");
            }

            var booking = await BookingQuery()
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return ApiResponse<CreatePayOsPaymentResponseDto>.Fail("Booking not found.");
            }

            if (booking.UserProfile.UserId != userId)
            {
                return ApiResponse<CreatePayOsPaymentResponseDto>.Fail("You are not authorized to pay this booking.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return ApiResponse<CreatePayOsPaymentResponseDto>.Fail("Only pending bookings can be paid.");
            }

            var existingSuccess = await _dbContext.Payments.AnyAsync(p =>
                p.BookingId == booking.Id &&
                p.Provider == PayOsProvider &&
                p.Type == PaymentType.BookingPayment &&
                p.Status == PaymentStatus.Success);
            if (existingSuccess)
            {
                return ApiResponse<CreatePayOsPaymentResponseDto>.Fail("This booking has already been paid.");
            }

            if (!TryGetPayOsAmount(booking.TotalPrice, out var amount))
            {
                return ApiResponse<CreatePayOsPaymentResponseDto>.Fail("PayOS amount must be a positive whole VND amount.");
            }

            var existingPending = await _dbContext.Payments
                .Where(p =>
                    p.BookingId == booking.Id &&
                    p.Provider == PayOsProvider &&
                    p.Type == PaymentType.BookingPayment &&
                    p.Status == PaymentStatus.Pending)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            var orderCode = existingPending is not null && long.TryParse(existingPending.TransactionCode, out var code)
                ? code
                : CreateOrderCode(booking.Id);

            var payment = existingPending ?? new Payment
            {
                UserId = userId,
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                Provider = PayOsProvider,
                TransactionCode = orderCode.ToString(),
                Type = PaymentType.BookingPayment,
                Status = PaymentStatus.Pending,
                Currency = "VND",
                CreatedAt = DateTimeOffset.Now
            };

            if (existingPending is null)
            {
                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync();
            }

            var result = await _payOsGateway.CreatePaymentLinkAsync(new PayOsCreatePaymentLinkRequest(
                orderCode,
                amount,
                CreateDescription(booking.Id),
                AppendBookingId(_settings.ReturnUrl, booking.Id),
                AppendBookingId(_settings.CancelUrl, booking.Id)));

            payment.ProviderPayload = JsonSerializer.Serialize(new
            {
                result.PaymentLinkId,
                result.CheckoutUrl,
                orderCode
            });
            payment.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<CreatePayOsPaymentResponseDto>.Ok(new CreatePayOsPaymentResponseDto
            {
                PaymentId = payment.Id,
                BookingId = booking.Id,
                OrderCode = orderCode,
                Amount = payment.Amount,
                Currency = payment.Currency,
                CheckoutUrl = result.CheckoutUrl,
                PaymentLinkId = result.PaymentLinkId,
                Status = payment.Status.ToString(),
                CreatedAt = payment.CreatedAt
            }, "PayOS payment link created successfully.");
        }

        public async Task<ApiResponse<PaymentResponseDto>> SyncPayOsPaymentAsync(int userId, int bookingId)
        {
            var paymentResult = await GetLatestPayOsPaymentForBookingAsync(userId, bookingId, requireOwner: true);
            if (!paymentResult.Success)
            {
                return ApiResponse<PaymentResponseDto>.Fail(paymentResult.Message);
            }

            var payment = paymentResult.Payment!;
            if (payment.Status == PaymentStatus.Success)
            {
                return ApiResponse<PaymentResponseDto>.Ok(MapToDto(payment), "Payment is already successful.");
            }

            if (!long.TryParse(payment.TransactionCode, out var orderCode))
            {
                return ApiResponse<PaymentResponseDto>.Fail("PayOS order code is invalid.");
            }

            var status = await _payOsGateway.GetPaymentLinkInformationAsync(orderCode);
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var previousStatus = payment.Status;
                    ApplyPayOsStatus(payment, status.Status, status.Reference, status.PaymentLinkId, status.RawPayload);
                    await AddPaymentSuccessNotificationAsync(payment, previousStatus);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return ApiResponse<PaymentResponseDto>.Ok(MapToDto(payment), "Payment synced successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<PaymentResponseDto>>> GetBookingPaymentsAsync(
            int userId,
            int bookingId)
        {
            var booking = await BookingQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return ApiResponse<IReadOnlyCollection<PaymentResponseDto>>.Fail("Booking not found.");
            }

            if (!await CanAccessBookingAsync(userId, booking))
            {
                return ApiResponse<IReadOnlyCollection<PaymentResponseDto>>.Fail("You are not authorized to view this booking payment.");
            }

            var payments = await _dbContext.Payments
                .AsNoTracking()
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .Select(p => MapToDto(p))
                .ToListAsync();

            return ApiResponse<IReadOnlyCollection<PaymentResponseDto>>.Ok(payments, "Payments retrieved successfully.");
        }

        public async Task<ApiResponse<PaymentResponseDto>> HandlePayOsWebhookAsync(JsonElement webhookBody)
        {
            var rawBody = webhookBody.GetRawText();
            var webhook = await _payOsGateway.VerifyWebhookAsync(rawBody);
            var orderCode = webhook.OrderCode.ToString();
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p =>
                    p.Provider == PayOsProvider &&
                    p.Type == PaymentType.BookingPayment &&
                    p.TransactionCode == orderCode);
            if (payment is null)
            {
                return ApiResponse<PaymentResponseDto>.Fail("Payment not found.");
            }

            var status = webhook.Success ? "PAID" : "FAILED";
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var previousStatus = payment.Status;
                    ApplyPayOsStatus(payment, status, webhook.Reference, webhook.PaymentLinkId, webhook.RawPayload);
                    await AddPaymentSuccessNotificationAsync(payment, previousStatus);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return ApiResponse<PaymentResponseDto>.Ok(MapToDto(payment), "PayOS webhook processed successfully.");
        }

        private IQueryable<Booking> BookingQuery()
        {
            return _dbContext.Bookings
                .Include(b => b.UserProfile)
                .Include(b => b.Court)
                    .ThenInclude(c => c.Venue)
                        .ThenInclude(v => v.CourtOwnerProfile)
                            .ThenInclude(cop => cop.UserProfile);
        }

        private async Task<(bool Success, string Message, Payment? Payment)> GetLatestPayOsPaymentForBookingAsync(
            int userId,
            int bookingId,
            bool requireOwner)
        {
            var booking = await BookingQuery().FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return (false, "Booking not found.", null);
            }

            if (requireOwner && booking.UserProfile.UserId != userId)
            {
                return (false, "You are not authorized to sync this booking payment.", null);
            }

            var payment = await _dbContext.Payments
                .Where(p =>
                    p.BookingId == bookingId &&
                    p.Provider == PayOsProvider &&
                    p.Type == PaymentType.BookingPayment)
                .OrderByDescending(p => p.Status == PaymentStatus.Success)
                .ThenByDescending(p => p.Id)
                .FirstOrDefaultAsync();
            if (payment is null)
            {
                return (false, "Payment not found.", null);
            }

            return (true, "Payment found.", payment);
        }

        private async Task<bool> CanAccessBookingAsync(int userId, Booking booking)
        {
            if (booking.UserProfile.UserId == userId || IsVenueOwner(userId, booking))
            {
                return true;
            }

            return await _dbContext.Users.AnyAsync(u => u.Id == userId && u.Role == UserRole.Admin);
        }

        private static bool IsVenueOwner(int userId, Booking booking)
        {
            return booking.Court.Venue.CourtOwnerProfile.UserProfile.UserId == userId;
        }

        private void ApplyPayOsStatus(
            Payment payment,
            string payOsStatus,
            string? reference,
            string? paymentLinkId,
            string rawPayload)
        {
            var normalizedStatus = payOsStatus.Trim().ToUpperInvariant();
            payment.ProviderPayload = rawPayload;
            payment.Note = BuildNote(reference, paymentLinkId);
            payment.UpdatedAt = DateTimeOffset.Now;

            if (normalizedStatus is "PAID" or "SUCCESS")
            {
                payment.Status = PaymentStatus.Success;
                payment.PaidAt ??= DateTimeOffset.Now;
                return;
            }

            if (normalizedStatus is "CANCELLED" or "EXPIRED" or "FAILED")
            {
                payment.Status = PaymentStatus.Failed;
            }
        }

        private bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(_settings.ClientId) &&
                !string.IsNullOrWhiteSpace(_settings.ApiKey) &&
                !string.IsNullOrWhiteSpace(_settings.ChecksumKey) &&
                !string.IsNullOrWhiteSpace(_settings.ReturnUrl) &&
                !string.IsNullOrWhiteSpace(_settings.CancelUrl);
        }

        private static bool TryGetPayOsAmount(decimal totalPrice, out long amount)
        {
            amount = 0;
            if (totalPrice <= 0 || totalPrice != decimal.Truncate(totalPrice))
            {
                return false;
            }

            if (totalPrice > long.MaxValue)
            {
                return false;
            }

            amount = decimal.ToInt64(totalPrice);
            return true;
        }

        private static long CreateOrderCode(int bookingId)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 1_000_000_000;
            return bookingId * 1_000_000_000L + timestamp;
        }

        private static string CreateDescription(int bookingId)
        {
            return $"Booking {bookingId}";
        }

        private static string AppendBookingId(string url, int bookingId)
        {
            var separator = url.Contains('?', StringComparison.Ordinal) ? "&" : "?";
            return $"{url}{separator}bookingId={bookingId}";
        }

        private static string? BuildNote(string? reference, string? paymentLinkId)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(reference))
            {
                parts.Add($"PayOS reference: {reference}");
            }

            if (!string.IsNullOrWhiteSpace(paymentLinkId))
            {
                parts.Add($"PayOS paymentLinkId: {paymentLinkId}");
            }

            return parts.Count == 0 ? null : string.Join("; ", parts);
        }

        /// <summary>
        /// Adds a payment success notification only when the payment transitions
        /// to Success from a non-Success state and no duplicate notification exists.
        /// </summary>
        private async Task AddPaymentSuccessNotificationAsync(Payment payment, PaymentStatus previousStatus)
        {
            if (previousStatus == PaymentStatus.Success || payment.Status != PaymentStatus.Success)
            {
                return;
            }

            await AcquirePaymentSuccessNotificationLockAsync(payment.Id);

            var exists = await _dbContext.Notifications.AnyAsync(n =>
                n.UserId == payment.UserId &&
                n.Type == NotificationType.Payment &&
                n.ReferenceType == NotificationReferenceType.Payment &&
                n.ReferenceId == payment.Id);

            if (!exists)
            {
                _notificationWriter.Add(new CreateNotificationRequest
                {
                    UserId = payment.UserId,
                    Title = "Thanh toán thành công",
                    Content = "Thanh toán cho đơn đặt sân của bạn đã được xác nhận thành công.",
                    Type = NotificationType.Payment,
                    ReferenceType = NotificationReferenceType.Payment,
                    ReferenceId = payment.Id
                });
            }
        }

        private async Task AcquirePaymentSuccessNotificationLockAsync(int paymentId)
        {
            if (!_dbContext.Database.IsRelational())
            {
                return;
            }

            var currentTransaction = _dbContext.Database.CurrentTransaction
                ?? throw new InvalidOperationException("Payment success notification lock requires an active transaction.");

            var connection = _dbContext.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.Transaction = currentTransaction.GetDbTransaction();
            command.CommandText = """
                DECLARE @result int;
                EXEC @result = sp_getapplock
                    @Resource = @resource,
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 10000;
                SELECT @result;
                """;

            var resourceParameter = command.CreateParameter();
            resourceParameter.ParameterName = "@resource";
            resourceParameter.Value = $"payment-success-notification:{paymentId}";
            command.Parameters.Add(resourceParameter);

            var result = (int)(await command.ExecuteScalarAsync()
                ?? throw new InvalidOperationException("Could not acquire payment success notification lock."));

            if (result < 0)
            {
                throw new InvalidOperationException("Could not acquire payment success notification lock.");
            }
        }

        private static PaymentResponseDto MapToDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Provider = payment.Provider,
                TransactionCode = payment.TransactionCode,
                Type = payment.Type.ToString(),
                Status = payment.Status.ToString(),
                PaidAt = payment.PaidAt,
                Currency = payment.Currency,
                Note = payment.Note,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }
    }
}
