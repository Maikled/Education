using Contracts.Errors;

namespace Core.Departments.Exceptions
{
    public class DepartmentParentConflictException : AppException
    {
        public DepartmentParentConflictException(Guid departmentId, Guid parentDepartmentId) : base(AppError.Conflict("department.parentconflict", $"Department with ID {departmentId} cannot have parent department with ID {parentDepartmentId} due to a conflict."))
        {
        }
    }
}
