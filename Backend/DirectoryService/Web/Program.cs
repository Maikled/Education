using Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using System.Globalization;
using Web.Middlewares;
using Web.Providers;

namespace Web
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web application");

                var builder = WebApplication.CreateBuilder(args);

                DotEnv.Load();

                builder.Services.AddLoggingServices(builder.Configuration);
                builder.Services.AddOpenApi();
                builder.Services.AddHealthChecks();
                builder.Services.AddWebServices(builder);

                var app = builder.Build();

                app.UseAppRequestLogging();
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
            catch (Exception ex) when (ex is not HostAbortedException)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
