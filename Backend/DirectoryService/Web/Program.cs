using Scalar.AspNetCore;
using Web.Middlewares;
using Web.Providers;

namespace Web
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            DotEnv.Load();

            var connectionString = DotEnv.Expand(builder.Configuration.GetConnectionString("PostgresConnection") ?? string.Empty);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'PostgresConnection' is not set. ");
            }

            builder.Services.AddSingleton<ILoggerFactory>((_) => LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));
            builder.Services.AddOpenApi();
            builder.Services.AddHealthChecks();
            builder.Services.AddWebServices(connectionString);

            var app = builder.Build();

            app.UseAppExceptionHandler();

            EndpointsProvider.RegisterAppEndpoints(app.MapGroup("/"));

            app.MapHealthChecks("/health");

            if (!app.Environment.IsProduction())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
                app.MapGet("/", () => Results.Redirect("/scalar"));
            }

            await app.RunAsync();
        }
    }
}
