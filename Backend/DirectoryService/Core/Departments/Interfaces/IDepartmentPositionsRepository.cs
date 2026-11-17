using Domain.Entities.AssociativeEntities;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentPositionsRepository
    {
        public Task<DepartmentPosition?> GetAsync(Guid departmentId, Guid positionId, CancellationToken cancellationToken);
        public Task<bool> ExistByPositionIdAsync(Guid positionId, CancellationToken cancellationToken);
        public Task AddAsync(DepartmentPosition departmentPosition, CancellationToken cancellationToken);
        public Task RemoveAsync(DepartmentPosition departmentPosition, CancellationToken cancellationToken);
        public Task RemoveByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken);
    }
}
