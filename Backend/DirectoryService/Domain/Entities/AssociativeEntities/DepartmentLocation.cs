using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Domain.Entities.AssociativeEntities
{
    public class DepartmentLocation
    {
        public Guid Id { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid LocationId { get; private set; }
        public bool IsPrimary { get; private set; }

        private DepartmentLocation(Guid departmentId, Guid locationId, bool isPrimary)
        {
            Id = Guid.CreateVersion7();
            DepartmentId = departmentId;
            LocationId = locationId;
            IsPrimary = isPrimary;
        }

        public static Result<DepartmentLocation, AppError> Create(Guid departmentId, Guid locationId, bool isPrimary)
        {
            if (departmentId == Guid.Empty)
            {
                return AppError.Validation("department_location", "DepartmentId cannot be an empty GUID.");
            }
            if (locationId == Guid.Empty)
            {
                return AppError.Validation("department_location", "LocationId cannot be an empty GUID.");
            }

            return new DepartmentLocation(departmentId, locationId, isPrimary);
        }
    }
}
