namespace PlayCourt.Application.Settings;

public sealed class BookingExpirationSettings
{
    public int PendingPaymentTimeoutMinutes { get; set; } = 15;
    public int ScanIntervalSeconds { get; set; } = 60;
    public int BatchSize { get; set; } = 100;

    public TimeSpan PendingPaymentTimeout =>
        TimeSpan.FromMinutes(Math.Max(1, PendingPaymentTimeoutMinutes));

    public TimeSpan ScanInterval =>
        TimeSpan.FromSeconds(Math.Max(5, ScanIntervalSeconds));

    public int NormalizedBatchSize => Math.Clamp(BatchSize, 1, 500);
}
