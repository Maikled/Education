using Serilog;
using Serilog.Events;

namespace Web.Middlewares
{
    internal static class RequestLoggingExtensions
    {
        public static IApplicationBuilder UseAppRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseSerilogRequestLogging(options => options.GetLevel = GetLevel);
        }

        private static LogEventLevel GetLevel(HttpContext context, double elapsedMilliseconds, Exception? exception)
        {
            if (exception != null || context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                return LogEventLevel.Error;

            if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
                return LogEventLevel.Warning;

            if (context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
                return LogEventLevel.Verbose;

            return LogEventLevel.Information;
        }
    }
}
