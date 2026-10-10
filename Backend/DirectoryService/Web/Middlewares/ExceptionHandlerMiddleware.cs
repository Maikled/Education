using Contracts.Errors;
using Contracts.Errors.Enums;

namespace Web.Middlewares
{
    public class ExceptionHandlerMiddleware : IMiddleware
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
                await HandleExceptionAsync(context, appException.AppError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, AppError.Internal("server.unexpected", ex.Message));
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, AppError appError)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = appError.Type switch
            {
                AppErrorType.Validation => StatusCodes.Status400BadRequest,
                AppErrorType.NotFound => StatusCodes.Status404NotFound,
                AppErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsJsonAsync(appError);
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
