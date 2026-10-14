using Contracts.Errors;

namespace Core.Departments.Exceptions
{
    public class DepartmentLocationNotExistException : AppException
    {
        public DepartmentLocationNotExistException(Guid departmentId, Guid locationId) : base(AppError.NotFound("department.location.notfound", $"Department location with IDs {departmentId} and {locationId} does not exist."))
        {
            
        }
    }
}
