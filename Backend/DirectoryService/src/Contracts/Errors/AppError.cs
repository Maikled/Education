using Contracts.Errors.Enums;

namespace Contracts.Errors
{
    public record AppError
    {
        public string Code { get; init; }
        public AppErrorType Type { get; init; }
        public string Message { get; init; }

        private AppError(string code, AppErrorType type, string message)
        {
            Code = code;
            Type = type;
            Message = message;
        }

        public static AppError Validation(string code, string message) => new AppError(code, AppErrorType.Validation, message); 
        public static AppError NotFound(string code, string message) => new AppError(code, AppErrorType.NotFound, message);
        public static AppError Conflict(string code, string message) => new AppError(code, AppErrorType.Conflict, message);
        public static AppError Internal(string code, string message) => new AppError(code, AppErrorType.Internal, message);
    }
}
