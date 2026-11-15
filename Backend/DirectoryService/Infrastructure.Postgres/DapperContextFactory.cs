using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace Infrastructure.Postgres
{
    internal class DapperContextFactory : IDisposable
    {
        private readonly NpgsqlDataSource _dataSource;

        public DapperContextFactory(string connectionString, ILoggerFactory loggerFactory)
        {
            _dataSource = new NpgsqlDataSourceBuilder(connectionString).UseLoggerFactory(loggerFactory).Build();
        }

        public async Task<IDbConnection> GetConnection(CancellationToken cancellationToken = default)
        {
            return await _dataSource.OpenConnectionAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dataSource.Dispose();
        }
    }
}
