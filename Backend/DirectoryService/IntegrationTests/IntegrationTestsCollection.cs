namespace IntegrationTests
{
    /// <summary>
    /// Общая коллекция для всех классов интеграционных тестов. Тестовые классы внутри одной коллекции
    /// выполняются последовательно: каждый получает свой <see cref="TestsFactory"/> (IClassFixture),
    /// и контейнеры с фиксированным именем "directory_service_test" не конфликтуют между собой.
    /// </summary>
    [CollectionDefinition(Name)]
    public sealed class IntegrationTestsCollection
    {
        public const string Name = "Integration tests";
    }
}
