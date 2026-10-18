using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Domain.Entities.AssociativeEntities
{
    public class DepartmentPosition
    {
        public Guid Id { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid PositionId { get; private set; }

        private DepartmentPosition(Guid departmentId, Guid positionId)
        {
            Id = Guid.CreateVersion7();
            DepartmentId = departmentId;
            PositionId = positionId;
        }

        public static Result<DepartmentPosition, AppError> Create(Guid departmentId, Guid positionId)
        {
            var validationResult = Validate(departmentId, positionId);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<DepartmentPosition>();

            return new DepartmentPosition(departmentId, positionId);
        }

        public static UnitResult<AppError> Validate(Guid departmentId, Guid positionId)
        {
            return Result.Combine(errors => AppError.FromErrors(errors), ValidateDepartmentId(departmentId), ValidatePositionId(positionId));
        }

        private static UnitResult<AppError> ValidateDepartmentId(Guid departmentId)
        {
            return UnitResult.FailureIf(departmentId == Guid.Empty, AppError.Validation("department.id", "DepartmentId cannot be an empty GUID."));
        }

        private static UnitResult<AppError> ValidatePositionId(Guid positionId)
        {
            return UnitResult.FailureIf(positionId == Guid.Empty, AppError.Validation("position.id", "PositionId cannot be an empty GUID."));
        }
    }
}
