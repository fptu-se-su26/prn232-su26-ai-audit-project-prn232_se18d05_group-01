using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlayCourt.Application.Interfaces;
using PlayCourt.Application.Settings;

namespace PlayCourt.Infrastructure.Services;

public sealed class BookingExpirationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingExpirationWorker> _logger;
    private readonly BookingExpirationSettings _settings;

    public BookingExpirationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingExpirationWorker> logger,
        IOptions<BookingExpirationSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_settings.ScanInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ExpireOnceAsync(stoppingToken);

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ExpireOnceAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBookingExpirationService>();
            var expiredCount = await service.ExpirePendingBookingsAsync(DateTimeOffset.Now, stoppingToken);

            if (expiredCount > 0)
            {
                _logger.LogInformation("Expired {ExpiredCount} pending bookings.", expiredCount);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to expire pending bookings.");
        }
    }
}
