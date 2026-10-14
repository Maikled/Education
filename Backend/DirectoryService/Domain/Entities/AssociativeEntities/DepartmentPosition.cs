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
            if (departmentId == Guid.Empty)
            {
                return AppError.Validation("department.id", "DepartmentId cannot be an empty GUID.");
            }
            if (positionId == Guid.Empty)
            {
                return AppError.Validation("position.id", "PositionId cannot be an empty GUID.");
            }

            return new DepartmentPosition(departmentId, positionId);
        }
    }
}
