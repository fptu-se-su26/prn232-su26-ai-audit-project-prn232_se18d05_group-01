using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PlayCourt.Application.Interfaces;
using PlayCourt.Application.Settings;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task SendAsync(string to, string subject, string body)
        {
            return SendAsync(to, subject, body, CancellationToken.None);
        }

        public Task SendVerifyEmailAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            const string subject = "Verify your PlayCourt email";
            var body = $"""
                <h2>Verify your PlayCourt account</h2>
                <p>Your verification code is:</p>
                <h1>{otp}</h1>
                <p>This code will expire in 10 minutes.</p>
                <p>If you did not create this account, please ignore this email.</p>
                <p>Do not share this code with anyone.</p>
                """;

            return SendAsync(toEmail, subject, body, cancellationToken);
        }

        public Task SendResetPasswordEmailAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            const string subject = "Reset your PlayCourt password";
            var body = $"""
                <h2>Reset your PlayCourt password</h2>
                <p>Your password reset code is:</p>
                <h1>{otp}</h1>
                <p>This code will expire in 10 minutes.</p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>Do not share this code with anyone.</p>
                """;

            return SendAsync(toEmail, subject, body, cancellationToken);
        }

        private async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

            using var client = new SmtpClient();
            var secureSocketOptions = _settings.EnableSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.Auto;

            await client.ConnectAsync(_settings.Host, _settings.Port, secureSocketOptions, cancellationToken);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
