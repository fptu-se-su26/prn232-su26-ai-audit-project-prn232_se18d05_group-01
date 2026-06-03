using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayCourt.Application.Interfaces;
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
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IVerificationTokenService, VerificationTokenService>();
            services.AddScoped<IEmailService, DevelopmentEmailService>();

            return services;
        }
    }
}
