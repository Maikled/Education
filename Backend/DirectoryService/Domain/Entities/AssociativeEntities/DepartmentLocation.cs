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
            var validationResult = Validate(departmentId, locationId);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<DepartmentLocation>();

            return new DepartmentLocation(departmentId, locationId, isPrimary);
        }

        public static UnitResult<AppError> Validate(Guid departmentId, Guid locationId)
        {
            return Result.Combine(errors => AppError.FromErrors(errors), ValidateDepartmentId(departmentId), ValidateLocationId(locationId));
        }

        private static UnitResult<AppError> ValidateDepartmentId(Guid departmentId)
        {
            return UnitResult.FailureIf(departmentId == Guid.Empty, AppError.Validation("department_location", "DepartmentId cannot be an empty GUID."));
        }

        private static UnitResult<AppError> ValidateLocationId(Guid locationId)
        {
            return UnitResult.FailureIf(locationId == Guid.Empty, AppError.Validation("department_location", "LocationId cannot be an empty GUID."));
        }
    }
}
