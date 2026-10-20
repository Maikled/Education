using Core;
using Infrastructure.Postgres;
using Serilog;
using Web.Middlewares;

namespace Web
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IHostApplicationBuilder builder)
        {
            services.AddCoreServices();

            var connectionString = DotEnv.Expand(builder.Configuration.GetConnectionString("PostgresConnection") ?? string.Empty);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'PostgresConnection' is not set. ");
            }

            services.AddInfrastructurePostgresServices(connectionString);


            services.AddTransient<ExceptionHandlerMiddleware>();

            return services;
        }

        public static IServiceCollection AddLoggingServices(this IServiceCollection services, IConfiguration configuration)
        {
            var placeholders = configuration.GetSection("Serilog")
                .AsEnumerable()
                .Where(pair => pair.Value?.Contains("${", StringComparison.Ordinal) == true)
                .ToList();

            foreach (var (key, value) in placeholders)
            {
                configuration[key] = DotEnv.Expand(value!);
            }

            services.AddSerilog((serviceProvider, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(serviceProvider)
                .Enrich.FromLogContext());

            return services;
        }
    }
}
