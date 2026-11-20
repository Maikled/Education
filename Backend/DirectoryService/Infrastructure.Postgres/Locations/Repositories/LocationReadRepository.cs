using System.Data;
using System.Diagnostics.CodeAnalysis;
using Contracts;
using Contracts.DTOs;
using Core.Enums;
using Core.Locations.GetLocations.Enums;
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

        public async Task<IEnumerable<TopLocationDto>> GetTopLocationsAsync(int limit, CancellationToken cancellationToken)
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
                ORDER BY COUNT(dl."Id") DESC, l."Id"
                LIMIT @Limit
                """;

            using var connection = await _contextFactory.GetConnection(cancellationToken);

            return await connection.QueryAsync<TopLocationDto>(
                new CommandDefinition(sql, new { Limit = limit }, cancellationToken: cancellationToken));
        }

        [SuppressMessage(
            "Security",
            "S2077:Formatting SQL queries is security-sensitive",
            Justification = "WHERE and ORDER BY are assembled only from constant fragments chosen by enum/flags; all user values are passed as parameters.")]
        public async Task<PagedResult<LocationListItemDto>> GetLocationsAsync(string? search, int? minDepartments, LocationSortFields? sortField, SortDirection sortDirection, int page, int pageSize, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            var conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                conditions.Add("l.name ILIKE '%' || @Search || '%'");
                parameters.Add("Search", search, DbType.String);
            }

            if (minDepartments.HasValue)
            {
                conditions.Add("COALESCE(dc.cnt, 0) >= @MinDepartments");
                parameters.Add("MinDepartments", minDepartments.Value, DbType.Int32);
            }

            parameters.Add("PageSize", pageSize, DbType.Int32);
            parameters.Add("Offset", (page - 1) * pageSize, DbType.Int32);

            var whereClause = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            var column = sortField switch
            {
                LocationSortFields.CreatedAt => "l.created_at",
                LocationSortFields.DepartmentsCount => "COALESCE(dc.cnt, 0)",
                _ => "l.name"
            };

            var direction = sortDirection == SortDirection.Descending ? "DESC" : "ASC";

            var orderByClause = $"ORDER BY {column} {direction}, l.\"Id\"";

            var sql = $"""
                WITH department_counts AS (
                    SELECT location_id, COUNT(*)::int AS cnt
                    FROM department_locations
                    GROUP BY location_id
                )
                SELECT
                    l."Id"               AS Id,
                    l.name               AS Name,
                    l."Address"          AS Address,
                    l.created_at         AS CreatedAt,
                    COALESCE(dc.cnt, 0)  AS DepartmentCount,
                    COUNT(*) OVER()::int AS TotalCount
                FROM locations l
                LEFT JOIN department_counts dc ON dc.location_id = l."Id"
                {whereClause}
                {orderByClause}
                LIMIT @PageSize OFFSET @Offset
                """;

            using var connection = await _contextFactory.GetConnection(cancellationToken);

            var totalCount = 0;

            var items = await connection.QueryAsync<LocationListItemDto, int, LocationListItemDto>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken),
                map: (location, total) =>
                {
                    totalCount = total;
                    return location;
                },
                splitOn: "TotalCount");

            return new PagedResult<LocationListItemDto>(items.ToList(), totalCount, page, pageSize);
        }
    }
}
