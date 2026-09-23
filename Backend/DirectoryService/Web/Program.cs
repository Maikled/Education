using Scalar.AspNetCore;

namespace Web
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.MapHealthChecks("/health");

            if (!app.Environment.IsProduction())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            await app.RunAsync();
        }
    }
}
