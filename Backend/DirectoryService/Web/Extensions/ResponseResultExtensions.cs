using Contracts.Errors;
using Contracts.Errors.Enums;

namespace Web.Extensions
{
    internal static class ResponseResultExtensions
    {
        public static IResult ToResponse(this AppError error)
        {
            return error.Type switch
            {
                AppErrorType.Validation => Results.BadRequest(error),
                AppErrorType.NotFound => Results.NotFound(error),
                AppErrorType.Conflict => Results.Conflict(error),
                _ => Results.InternalServerError(error)
            };
        }
    }
}
