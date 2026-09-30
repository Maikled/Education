using Core;
using Infrastructure.Postgres;

namespace Web
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddCoreServices();
            services.AddInfrastructurePostgresServices();

            return services;
        }
    }
}
