namespace IntegrationTests
{
    public class DatabaseResetTestsFactory : IClassFixture<TestsFactory>, IAsyncLifetime
    {
        protected readonly TestsFactory TestsFactory;
        protected readonly HttpClient TestHttpClient;

        public DatabaseResetTestsFactory(TestsFactory testsFactory)
        {
            TestsFactory = testsFactory;
            TestHttpClient = testsFactory.CreateClient();
        }

        protected async Task ResetDatabaseAsync()
        {
            await TestsFactory.ResetDatabaseAsync();
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await ResetDatabaseAsync();
        }
    }
}
