using Core;
using Infrastructure.Postgres;

namespace Web
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, string connectionString)
        {
            services.AddCoreServices();
            services.AddInfrastructurePostgresServices(connectionString);

            return services;
        }
    }
}
