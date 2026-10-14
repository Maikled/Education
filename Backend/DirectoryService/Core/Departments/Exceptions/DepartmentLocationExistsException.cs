using Contracts.Errors;

namespace Core.Departments.Exceptions
{
    public class DepartmentLocationExistsException : AppException
    {
        public DepartmentLocationExistsException(Guid departmentId, Guid locationId) : base(AppError.Conflict("department.locations.exists", $"Department location with IDs {departmentId} and {locationId} already exists."))
        {
            
        }
    }
}
