using Microsoft.Extensions.Logging;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class DevelopmentEmailService : IEmailService
    {
        private readonly ILogger<DevelopmentEmailService> _logger;

        public DevelopmentEmailService(ILogger<DevelopmentEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body)
        {
            _logger.LogInformation(
                "Development email. To: {To}. Subject: {Subject}. Body: {Body}",
                to,
                subject,
                body);

            return Task.CompletedTask;
        }

        public Task SendVerifyEmailAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            return SendAsync(
                toEmail,
                "Verify your PlayCourt email",
                $"Your PlayCourt verification code is {otp}. This code will expire in 10 minutes.");
        }
    }
}
