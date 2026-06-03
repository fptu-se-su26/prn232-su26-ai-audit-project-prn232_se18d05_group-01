using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PlayCourt.API.Authorization;
using PlayCourt.API.Middlewares;

namespace PlayCourt.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký các service thuộc tầng API tại đây.
            // Ví dụ: controllers, Swagger, CORS, authentication, authorization.
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var jwtSection = configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var jwtIssuer = jwtSection["Issuer"] ?? "PlayCourt";
            var jwtAudience = jwtSection["Audience"] ?? "PlayCourtClient";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role
                    };
                });

            // Dang ky cac policy phan quyen theo role cua user.
            // API public thi khong can gan [Authorize].
            services.AddAuthorization(options =>
            {
                // Chi user co role Admin moi goi duoc API gan policy nay.
                options.AddPolicy(ApiPolicies.Admin, policy =>
                    policy.RequireRole(ApiPolicies.Admin));

                // Chi user co role Player moi goi duoc API gan policy nay.
                options.AddPolicy(ApiPolicies.Player, policy =>
                    policy.RequireRole(ApiPolicies.Player));

                // Chi user co role CourtOwner moi goi duoc API gan policy nay.
                options.AddPolicy(ApiPolicies.CourtOwner, policy =>
                    policy.RequireRole(ApiPolicies.CourtOwner));
            });

            return services;
        }

        public static WebApplication UseApiPipeline(this WebApplication app)
        {
            // Middleware dùng chung cho toàn bộ request pipeline.
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Bổ sung middleware mới ở đây theo đúng thứ tự pipeline của ASP.NET Core.
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
