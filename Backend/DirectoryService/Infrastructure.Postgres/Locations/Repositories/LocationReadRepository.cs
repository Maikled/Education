using Contracts.DTOs;
using Core.Locations.Interfaces;
using Dapper;

namespace Infrastructure.Postgres.Locations.Repositories
{
    internal class LocationReadRepository : ILocationReadRepository
    {
        private readonly DapperContextFactory _contextFactory;

        public LocationReadRepository(DapperContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<LocationDto?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT
                    l."Id"       AS Id,
                    l.name       AS Name,
                    l."Address"  AS Address,
                    l.created_at AS CreatedAt,
                    l.updated_at AS UpdatedAt
                FROM locations l
                WHERE l."Id" = @LocationId
                """;

            using var connection = await _contextFactory.GetConnection(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<LocationDto>(
                new CommandDefinition(sql, new { LocationId = locationId }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<TopLocationDTO>> GetTopLocationsAsync(int limit, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT
                    l."Id"       AS Id,
                    l.name       AS Name,
                    l."Address"  AS Address,
                    COUNT(dl."Id")::int AS DepartmentCount
                FROM locations l
                LEFT JOIN department_locations dl ON l."Id" = dl.location_id
                GROUP BY l."Id", l.name, l."Address"
                ORDER BY COUNT(dl."Id"), l."Id" DESC
                LIMIT @Limit
                """;

            using var connection = await _contextFactory.GetConnection(cancellationToken);

            return await connection.QueryAsync<TopLocationDTO>(
                new CommandDefinition(sql, new { Limit = limit }, cancellationToken: cancellationToken));
        }
    }
}
