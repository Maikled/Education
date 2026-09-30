using Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Web.Providers;

namespace Web
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            DotEnv.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();
            builder.Services.AddHealthChecks();
            builder.Services.AddWebServices();
            
            var connectionString = DotEnv.Expand(builder.Configuration.GetConnectionString("PostgresConnection") ?? string.Empty);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'PostgresConnection' is not set. ");
            }

            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            var app = builder.Build();

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
