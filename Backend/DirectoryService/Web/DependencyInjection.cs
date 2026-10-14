using Core;
using Infrastructure.Postgres;
using Web.Middlewares;

namespace Web
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, string connectionString)
        {
            services.AddCoreServices();
            services.AddInfrastructurePostgresServices(connectionString);

            services.AddTransient<ExceptionHandlerMiddleware>();

            return services;
        }
    }
}
