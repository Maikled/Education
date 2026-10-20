using Contracts.Errors;
using Web.Endpoints.Models;

namespace Web.Middlewares
{
    internal partial class ExceptionHandlerMiddleware : IMiddleware
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
                await new ErrorResult(appException.AppError).ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                LogUnexpectedError(ex);
                await new ErrorResult(AppError.Internal("server.unexpected", "Server unexpected error occurred.")).ExecuteAsync(context);
            }
        }

        [LoggerMessage(Level = LogLevel.Error, Message = "An unexpected error occurred")]
        private partial void LogUnexpectedError(Exception exception);
    }

    internal static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
