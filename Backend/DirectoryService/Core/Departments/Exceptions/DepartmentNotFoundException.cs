using Contracts.Errors;

namespace Core.Departments.Exceptions
{
    public class DepartmentNotFoundException : AppException
    {
        public DepartmentNotFoundException(Guid departmentId) : base(AppError.NotFound("department.notfound", $"Department with ID {departmentId} was not found."))
        {
        }
    }
}
