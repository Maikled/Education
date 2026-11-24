using Infrastructure.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;
using Web;

namespace IntegrationTests
{
    public class TestsFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("directory_service_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private Respawner? _respawner;
        private DbConnection? _dbConnection;

        public TestsFactory()
        {
            UseKestrel(0);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            // UseSetting применяется до того, как Program прочитает конфигурацию,
            // поэтому и EF, и DapperContextFactory получат строку контейнера
            builder.UseSetting("ConnectionStrings:PostgresConnection", _dbContainer.GetConnectionString());

            // Убираем Seq из логов в тестах (индекс 1 = Seq в WriteTo)
            builder.UseSetting("Serilog:WriteTo:1:Name", "Console");
        }

        public async ValueTask InitializeAsync()
        {
            await _dbContainer.StartAsync();

            await using var scope = Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            await _dbConnection.OpenAsync();

            await InitializeRespawner();
        }

        public override async ValueTask DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();

            if (_dbConnection != null)
            {
                await _dbConnection.CloseAsync();
                await _dbConnection.DisposeAsync();
            }

            await base.DisposeAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            if (_dbConnection != null && _respawner != null)
            {
                await _respawner.ResetAsync(_dbConnection);
            }
        }

        private async Task InitializeRespawner()
        {
            if (_dbConnection == null)
                throw new InvalidOperationException("Database connection is not initialized.");

            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions()
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            });
        }
    }
}
