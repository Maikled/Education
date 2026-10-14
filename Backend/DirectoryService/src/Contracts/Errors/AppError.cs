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

        private static AppError JoinErrors(AppError[] errors)
        {
            if (errors == null || errors.Length == 0)
                throw new ArgumentException("Errors array cannot be null or empty.", nameof(errors));

            var errorGroups = errors.Distinct().GroupBy(e => e.Type).ToList();
            if (errorGroups.Count > 1)
                throw new InvalidOperationException("Cannot join errors of different types.");

            var firstGroup = errorGroups[0];

            return new AppError(string.Join(';', firstGroup.Select(e => e.Code)), firstGroup.Key, string.Join(';', firstGroup.Select(e => e.Message)));
        }

        public static AppError Validation(string code, string message) => new AppError(code, AppErrorType.Validation, message);
        public static AppError NotFound(string code, string message) => new AppError(code, AppErrorType.NotFound, message);
        public static AppError Conflict(string code, string message) => new AppError(code, AppErrorType.Conflict, message);
        public static AppError Internal(string code, string message) => new AppError(code, AppErrorType.Internal, message);
        public static AppError FromErrors(AppError[] errors) => JoinErrors(errors);
    }
}
