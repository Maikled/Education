using Core.Positions.Interfaces;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Positions.Repositories
{
    internal class PositionsRepository : IPositionsRepository
    {
        private readonly AppDbContext _dbContext;

        public PositionsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddAsync(Position position, CancellationToken cancellationToken)
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);

            return position.Id;
        }

        public async Task<bool> ExistWithNameAsync(Name name, CancellationToken cancellationToken)
        {
            return await _dbContext.Positions.Where(p => p.Name == name).AnyAsync(cancellationToken);
        }

        public async Task<Position?> GetByIdAsync(Guid positionId, CancellationToken cancellationToken)
        {
            return await _dbContext.Positions.Where(p => p.Id == positionId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task RemoveAsync(Position position, CancellationToken cancellationToken)
        {
            _dbContext.Positions.Remove(position);
        }
    }
}
