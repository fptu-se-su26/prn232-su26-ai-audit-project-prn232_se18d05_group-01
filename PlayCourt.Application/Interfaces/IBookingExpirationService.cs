namespace PlayCourt.Application.Interfaces;

public interface IBookingExpirationService
{
    Task<int> ExpirePendingBookingsAsync(DateTimeOffset now, CancellationToken cancellationToken = default);
}
