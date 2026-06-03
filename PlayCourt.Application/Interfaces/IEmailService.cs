namespace PlayCourt.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
        Task SendVerifyEmailAsync(string toEmail, string otp, CancellationToken cancellationToken = default);
    }
}
