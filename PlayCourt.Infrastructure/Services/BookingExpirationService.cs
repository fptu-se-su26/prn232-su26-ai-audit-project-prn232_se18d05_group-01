using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlayCourt.Application.DTOs.Notifications;
using PlayCourt.Application.Interfaces;
using PlayCourt.Application.Settings;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services;

public sealed class BookingExpirationService : IBookingExpirationService
{
    private const string PayOsProvider = "payOS";
    private const string ExpirationReason = "Booking expired because PayOS payment was not completed before timeout.";
    private const string PaymentExpiredNote = "Payment failed because booking expired.";
    private readonly PlayCourtDbContext _dbContext;
    private readonly INotificationWriter _notificationWriter;
    private readonly BookingExpirationSettings _settings;

    public BookingExpirationService(
        PlayCourtDbContext dbContext,
        INotificationWriter notificationWriter,
        IOptions<BookingExpirationSettings> settings)
    {
        _dbContext = dbContext;
        _notificationWriter = notificationWriter;
        _settings = settings.Value;
    }

    public async Task<int> ExpirePendingBookingsAsync(
        DateTimeOffset now,
        CancellationToken cancellationToken = default)
    {
        var cutoff = now - _settings.PendingPaymentTimeout;
        var bookingIds = await _dbContext.Bookings
            .Where(booking =>
                booking.Status == BookingStatus.Pending &&
                booking.CreatedAt <= cutoff)
            .OrderBy(booking => booking.CreatedAt)
            .Take(_settings.NormalizedBatchSize)
            .Select(booking => booking.Id)
            .ToListAsync(cancellationToken);

        var expiredCount = 0;
        foreach (var bookingId in bookingIds)
        {
            var expired = _dbContext.Database.IsRelational()
                ? await ExpireRelationalAsync(bookingId, cutoff, now, cancellationToken)
                : await ExpireTrackedAsync(bookingId, cutoff, now, cancellationToken);

            if (expired)
            {
                expiredCount++;
            }
        }

        return expiredCount;
    }

    private async Task<bool> ExpireRelationalAsync(
        int bookingId,
        DateTimeOffset cutoff,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var affected = await _dbContext.Bookings
                .Where(booking =>
                    booking.Id == bookingId &&
                    booking.Status == BookingStatus.Pending &&
                    booking.CreatedAt <= cutoff)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(booking => booking.Status, BookingStatus.Expired)
                    .SetProperty(booking => booking.UpdatedAt, now),
                    cancellationToken);

            if (affected == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            await _dbContext.Payments
                .Where(payment =>
                    payment.BookingId == bookingId &&
                    payment.Provider == PayOsProvider &&
                    payment.Type == PaymentType.BookingPayment &&
                    payment.Status == PaymentStatus.Pending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(payment => payment.Status, PaymentStatus.Failed)
                    .SetProperty(payment => payment.UpdatedAt, now)
                    .SetProperty(payment => payment.Note,
                        payment => payment.Note == null || payment.Note == string.Empty
                            ? PaymentExpiredNote
                            : payment.Note + "; " + PaymentExpiredNote),
                    cancellationToken);

            _dbContext.BookingStatusHistories.Add(CreateHistory(bookingId, now));
            await AddExpirationNotificationAsync(bookingId, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<bool> ExpireTrackedAsync(
        int bookingId,
        DateTimeOffset cutoff,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var booking = await _dbContext.Bookings
            .Include(item => item.Payments.Where(payment =>
                payment.Provider == PayOsProvider &&
                payment.Type == PaymentType.BookingPayment &&
                payment.Status == PaymentStatus.Pending))
            .FirstOrDefaultAsync(item =>
                item.Id == bookingId &&
                item.Status == BookingStatus.Pending &&
                item.CreatedAt <= cutoff,
                cancellationToken);

        if (booking is null)
        {
            return false;
        }

        booking.Status = BookingStatus.Expired;
        booking.UpdatedAt = now;
        foreach (var payment in booking.Payments)
        {
            payment.Status = PaymentStatus.Failed;
            payment.UpdatedAt = now;
            payment.Note = string.IsNullOrWhiteSpace(payment.Note)
                ? PaymentExpiredNote
                : $"{payment.Note}; {PaymentExpiredNote}";
        }

        _dbContext.BookingStatusHistories.Add(CreateHistory(booking.Id, now));
        await AddExpirationNotificationAsync(booking.Id, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task AddExpirationNotificationAsync(int bookingId, CancellationToken cancellationToken)
    {
        var info = await _dbContext.Bookings
            .Where(booking => booking.Id == bookingId)
            .Select(booking => new
            {
                booking.UserProfile.UserId,
                VenueName = booking.Court.Venue.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (info is null)
        {
            return;
        }

        var timeoutMinutes = (int)_settings.PendingPaymentTimeout.TotalMinutes;
        _notificationWriter.Add(new CreateNotificationRequest
        {
            UserId = info.UserId,
            Title = "Đơn đặt sân đã bị hủy",
            Content = $"Đơn đặt sân của bạn tại {info.VenueName} đã bị hủy do quá thời gian thanh toán ({timeoutMinutes} phút).",
            Type = NotificationType.Booking,
            ReferenceType = NotificationReferenceType.Booking,
            ReferenceId = bookingId
        });
    }

    private static BookingStatusHistory CreateHistory(int bookingId, DateTimeOffset now)
    {
        return new BookingStatusHistory
        {
            BookingId = bookingId,
            OldStatus = BookingStatus.Pending,
            NewStatus = BookingStatus.Expired,
            ChangedByUserId = null,
            Reason = ExpirationReason,
            CreatedAt = now
        };
    }
}
