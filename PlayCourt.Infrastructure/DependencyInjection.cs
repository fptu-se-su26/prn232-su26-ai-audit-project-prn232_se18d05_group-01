using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayCourt.Application.Interfaces;
using PlayCourt.Application.Settings;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Đăng ký DbContext và các dependency hạ tầng tại đây.
            // Ví dụ sau này có repository: services.AddScoped<ISportRepository, SportRepository>();
            services.AddDbContext<PlayCourtDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Dang ky service implementation o tang Infrastructure.
            // Interface nam o Application, class Service nam o Infrastructure.
            // Vi du khi co SportService: services.AddScoped<ISportService, SportService>();
            services.AddScoped<IService, Service>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICourtOwnerService, CourtOwnerService>();
            services.AddScoped<ISportService, SportService>();
            services.AddScoped<IVenueService, VenueService>();
            services.AddScoped<IAmenityService, AmenityService>();
            services.AddScoped<ICourtService, CourtService>();
            services.AddScoped<IPricingRuleService, PricingRuleService>();
            services.AddScoped<ICourtScheduleService, CourtScheduleService>();
            services.AddScoped<IMatchService, MatchService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IVerificationTokenService, VerificationTokenService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IVenueStaffService, VenueStaffService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPayOsGateway, PayOsGateway>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationWriter, NotificationWriter>();
            services.Configure<EmailSettings>(settings =>
            {
                var emailSection = configuration.GetSection("EmailSettings");
                settings.Host = emailSection["Host"] ?? string.Empty;
                settings.Port = int.TryParse(emailSection["Port"], out var port) ? port : 587;
                settings.UserName = emailSection["UserName"] ?? string.Empty;
                settings.Password = emailSection["Password"] ?? string.Empty;
                settings.FromEmail = emailSection["FromEmail"] ?? string.Empty;
                settings.FromName = emailSection["FromName"] ?? "PlayCourt";
                settings.EnableSsl = !bool.TryParse(emailSection["EnableSsl"], out var enableSsl) || enableSsl;
            });
            services.Configure<PayOsSettings>(settings =>
            {
                var payOsSection = configuration.GetSection("PayOs");
                settings.ClientId = payOsSection["ClientId"] ?? string.Empty;
                settings.ApiKey = payOsSection["ApiKey"] ?? string.Empty;
                settings.ChecksumKey = payOsSection["ChecksumKey"] ?? string.Empty;
                settings.ReturnUrl = payOsSection["ReturnUrl"] ?? string.Empty;
                settings.CancelUrl = payOsSection["CancelUrl"] ?? string.Empty;
            });
            services.AddScoped<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}
