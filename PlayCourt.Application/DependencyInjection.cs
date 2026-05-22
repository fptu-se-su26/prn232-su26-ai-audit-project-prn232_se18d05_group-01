using Microsoft.Extensions.DependencyInjection;

namespace PlayCourt.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Tang Application chua interface, DTO, common response, validation/use case logic neu co.
            // Service implementation dang duoc dat o tang Infrastructure va dang ky trong Infrastructure/DependencyInjection.cs.

            return services;
        }
    }
}
