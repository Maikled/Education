using Contracts.Errors;
using Web.Endpoints.Models;

namespace Web.Middlewares
{
    internal class ExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (AppException appException)
            {
                _logger.LogError(appException, "An application error occurred.");
                await new ErrorResult(appException.AppError).ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await new ErrorResult(AppError.Internal("server.unexpected", "Server unexpected error occurred.")).ExecuteAsync(context);
            }
        }
    }

    internal static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
