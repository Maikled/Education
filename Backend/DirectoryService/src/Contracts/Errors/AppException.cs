namespace Contracts.Errors
{
    public class AppException : Exception
    {
        public AppError AppError { get; }

        public AppException(AppError appError) : base(appError.Message)
        {
            AppError = appError;
        }
    }
}
