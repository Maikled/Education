using Domain.Entities.AssociativeEntities;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentLocationsRepository
    {
        public Task<DepartmentLocation?> GetAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
        public Task AddAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
        public Task RemoveAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
        public Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
