using Contracts.Errors;

namespace Core.Departments.Exceptions
{
    public class DepartmentValidationException : AppException
    {
        public DepartmentValidationException(IEnumerable<string> validationErrorsMessages) : base(AppError.Validation("department.validation", string.Join(", ", validationErrorsMessages)))
        {
        }
    }
}
