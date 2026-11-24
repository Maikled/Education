using Contracts.DTOs;
using Contracts.Errors.Enums;
using System.Net;

namespace IntegrationTests
{
    /// <summary>
    /// GET /positions и GET /positions/{id} пока заглушки, поэтому результат операций
    /// проверяется через побочные эффекты: конфликт имён, привязку к подразделению и удаление.
    /// </summary>
    [Collection(IntegrationTestsCollection.Name)]
    public class PositionTests : ApiTestsBase
    {
        public PositionTests(TestsFactory testsFactory) : base(testsFactory)
        {

        }

        #region CreatePosition

        [Fact]
        public async Task CreatePosition_with_valid_data_should_create_position()
        {
            var positionId = await CreatePositionAsync("  Developer  ");
            var departmentId = await CreateDepartmentAsync("Development", "development");

            // Позиция существует — её можно привязать к подразделению
            await AttachPositionAsync(departmentId, positionId);

            // Имя сохранено нормализованным — повтор без пробелов даёт конфликт
            AssertError(await PostAsync<Guid>("/positions/", new CreatePositionDto("Developer")), HttpStatusCode.Conflict, AppErrorType.Conflict, "position.name");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("A")]
        [InlineData("This position name is definitely longer than fifty characters")]
        public async Task CreatePosition_with_invalid_name_should_return_validation_error(string name)
        {
            var response = await PostAsync<Guid>("/positions/", new CreatePositionDto(name));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, "name.value");
        }

        [Fact]
        public async Task CreatePosition_with_existing_name_should_return_conflict()
        {
            await CreatePositionAsync("Developer");

            var response = await PostAsync<Guid>("/positions/", new CreatePositionDto("Developer"));

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "position.name");
        }

        #endregion

        #region UpdatePosition

        [Fact]
        public async Task UpdatePosition_with_valid_name_should_update_position()
        {
            var positionId = await CreatePositionAsync("Developer");

            AssertSuccess(await PatchAsync<object>($"/positions/{positionId}", new UpdatePositionDto("Senior developer")));

            // Старое имя освободилось, новое — занято
            await CreatePositionAsync("Developer");
            AssertError(await PostAsync<Guid>("/positions/", new CreatePositionDto("Senior developer")), HttpStatusCode.Conflict, AppErrorType.Conflict, "position.name");
        }

        [Fact]
        public async Task UpdatePosition_with_same_name_should_return_success()
        {
            var positionId = await CreatePositionAsync("Developer");

            AssertSuccess(await PatchAsync<object>($"/positions/{positionId}", new UpdatePositionDto("Developer")));
        }

        [Fact]
        public async Task UpdatePosition_with_not_existing_id_should_return_not_found()
        {
            var response = await PatchAsync<object>($"/positions/{Guid.NewGuid()}", new UpdatePositionDto("Developer"));

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "position");
        }

        [Fact]
        public async Task UpdatePosition_with_invalid_name_should_return_validation_error()
        {
            var positionId = await CreatePositionAsync("Developer");

            var response = await PatchAsync<object>($"/positions/{positionId}", new UpdatePositionDto("A"));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, "name.value");
        }

        [Fact]
        public async Task UpdatePosition_with_name_of_another_position_should_return_conflict()
        {
            await CreatePositionAsync("Developer");
            var positionId = await CreatePositionAsync("Tester");

            var response = await PatchAsync<object>($"/positions/{positionId}", new UpdatePositionDto("Developer"));

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "position.name");
        }

        #endregion

        #region DeletePosition

        [Fact]
        public async Task DeletePosition_with_existing_id_should_delete_position()
        {
            var positionId = await CreatePositionAsync("Developer");

            AssertSuccess(await DeleteAsync<object>($"/positions/{positionId}"));

            AssertError(await DeleteAsync<object>($"/positions/{positionId}"), HttpStatusCode.NotFound, AppErrorType.NotFound, "position");
            await CreatePositionAsync("Developer");
        }

        [Fact]
        public async Task DeletePosition_with_not_existing_id_should_return_not_found()
        {
            var response = await DeleteAsync<object>($"/positions/{Guid.NewGuid()}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "position");
        }

        [Fact]
        public async Task DeletePosition_attached_to_department_should_return_conflict()
        {
            var positionId = await CreatePositionAsync("Developer");
            var departmentId = await CreateDepartmentAsync("Development", "development");
            await AttachPositionAsync(departmentId, positionId);

            var response = await DeleteAsync<object>($"/positions/{positionId}");

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "position");
        }

        #endregion

        #region Arrange helpers

        private async Task<Guid> CreateDepartmentAsync(string name, string slug, Guid? parentId = null, IEnumerable<CreateDepartmentLocationDto>? locations = null)
        {
            var response = await PostAsync<Guid>("/departments/", new CreateDepartmentDto(name, slug, parentId, locations));
            var id = AssertSuccess(response, HttpStatusCode.Created);
            Assert.NotEqual(Guid.Empty, id);

            return id;
        }

        private async Task<Guid> CreatePositionAsync(string name)
        {
            var response = await PostAsync<Guid>("/positions/", new CreatePositionDto(name));
            var id = AssertSuccess(response, HttpStatusCode.Created);
            Assert.NotEqual(Guid.Empty, id);

            return id;
        }

        private async Task AttachPositionAsync(Guid departmentId, Guid positionId)
        {
            AssertSuccess(await PostAsync<object>($"/departments/{departmentId}/positions/{positionId}"));
        }

        #endregion
    }
}
