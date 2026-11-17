using Domain.Entities;
using Domain.Entities.ValueObjects;

namespace Core.Positions.Interfaces
{
    public interface IPositionsRepository
    {
        public Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);
        public Task<bool> ExistWithNameAsync(Name name, CancellationToken cancellationToken);
        public Task<Position?> GetByIdAsync(Guid positionId, CancellationToken cancellationToken);
        public Task RemoveAsync(Position position, CancellationToken cancellationToken);
    }
}
