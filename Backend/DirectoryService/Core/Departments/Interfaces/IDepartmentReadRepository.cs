using Domain.Entities;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentReadRepository
    {
        public Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);
    }
}
