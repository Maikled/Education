using Contracts.Errors;

namespace Core.Departments.Exceptions
{
    public class DepartmentsDescendantException : AppException
    {
        public DepartmentsDescendantException(Guid departmentId, Guid parentId) : base(AppError.Conflict("department.descendant.conflict", $"Department with ID {parentId} is a descendant of department with ID {departmentId} and cannot be its parent."))
        {
            
        }
    }
}
