using Core.Locations.Interfaces;
using Dapper;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Postgres.Locations.Repositories
{
    internal class DapperLocationsRepository : ILocationsRepository
    {
        private readonly DapperContextFactory _contextFactory;
        private readonly ILogger<DapperLocationsRepository> _logger;
        private const string InsertLocationSql = """INSERT INTO locations("Id", name, created_at, updated_at, "Address") VALUES (@Id, @Name, @CreatedAt, @UpdatedAt, @Address::jsonb);""";
        private const string CheckLocationExistenceSql = """SELECT EXISTS(SELECT * FROM locations WHERE name = @Name)""";
        private const string CheckLocationExistenceByIdSql = """SELECT EXISTS(SELECT * FROM locations WHERE "Id" = @Id)""";

        public DapperLocationsRepository(DapperContextFactory contextFactory, ILogger<DapperLocationsRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
        {
            try
            {
                var objectToInsert = new
                {
                    Id = location.Id,
                    Name = location.Name.Value,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt,
                    Address = JsonSerializer.Serialize(location.Address)
                };

                using var connection = await _contextFactory.GetConnection(cancellationToken);

                await connection.ExecuteAsync(InsertLocationSql, objectToInsert);

                return location.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the location with ID {LocationId}.", location.Id);
                throw;
            }
        }

        public async Task<bool> ExistWithNameAsync(Name name, CancellationToken cancellationToken)
        {
            using var connection = await _contextFactory.GetConnection(cancellationToken);

            var result = await connection.QueryAsync<bool>(CheckLocationExistenceSql, new { Name = name.Value });

            return result.First();
        }

        public async Task<bool> ExistById(Guid locationId, CancellationToken cancellationToken)
        {
            using var connection = await _contextFactory.GetConnection(cancellationToken);

            var result = await connection.QueryAsync<bool>(CheckLocationExistenceByIdSql, new { Id = locationId });

            return result.First();
        }
    }
}
