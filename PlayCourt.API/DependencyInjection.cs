using PlayCourt.API.Middlewares;

namespace PlayCourt.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            // Đăng ký các service thuộc tầng API tại đây.
            // Ví dụ: controllers, Swagger, CORS, authentication, authorization.
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
