using Contracts.Errors;
using Contracts.Errors.Enums;
using Serilog;

namespace Web.Endpoints.Models
{
    internal class ErrorResult : IResult
    {
        private readonly AppError _error;

        public ErrorResult(AppError error)
        {
            _error = error;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            var diagnosticContext = httpContext.RequestServices.GetService<IDiagnosticContext>();
            diagnosticContext?.Set("ErrorCode", _error.Code);
            diagnosticContext?.Set("ErrorType", _error.Type);

            httpContext.Response.StatusCode = _error.Type switch
            {
                AppErrorType.Validation => StatusCodes.Status400BadRequest,
                AppErrorType.NotFound => StatusCodes.Status404NotFound,
                AppErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return httpContext.Response.WriteAsJsonAsync(Envelope.Error(_error));
        }
    }
}
