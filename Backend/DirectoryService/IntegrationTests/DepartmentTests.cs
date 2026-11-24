using Contracts;
using Contracts.DTOs;
using Contracts.Errors.Enums;
using System.Net;
using System.Text;
using System.Text.Json;
using Web.Endpoints.Models;

namespace IntegrationTests
{
    [Collection(IntegrationTestsCollection.Name)]
    public class DepartmentTests : ApiTestsBase
    {
        public DepartmentTests(TestsFactory testsFactory) : base(testsFactory)
        {

        }

        #region GetDepartment

        [Fact]
        public async Task GetDepartment_with_not_existing_id_should_return_not_found()
        {
            var response = await TestHttpClient.GetAsync(new Uri($"/departments/{Guid.NewGuid()}", UriKind.Relative), CancellationToken.None);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var responseStringResponse = await response.Content.ReadAsStringAsync(CancellationToken.None);
            Assert.NotEmpty(responseStringResponse);

            var getDepartmentEnvelope = JsonSerializer.Deserialize<Envelope<DepartmentDto>>(responseStringResponse, JsonSerializerOptions.Web);
            Assert.NotNull(getDepartmentEnvelope);

            Assert.NotNull(getDepartmentEnvelope.Error);
            Assert.Equal(AppErrorType.NotFound, getDepartmentEnvelope.Error.Type);
            Assert.Equal("department", getDepartmentEnvelope.Error.Code);
        }

        [Fact]
        public async Task GetDepartment_with_existing_id_should_return_department()
        {
            var departmentId = await CreateDepartmentAsync("  Sales  ", "  SALES  ");

            var department = AssertSuccess(await GetAsync<DepartmentDto>($"/departments/{departmentId}"));

            Assert.Equal(departmentId, department.Id);
            Assert.Equal("Sales", department.Name);
            Assert.Equal("sales", department.Slug);
            Assert.Equal("sales", department.Path);
            Assert.Null(department.ParentId);
            Assert.Null(department.UpdatedAt);
        }

        #endregion

        #region CreateDepartment

        [Fact]
        public async Task CreateDepartment_with_no_locations_should_return_success()
        {
            var createDepartmentDto = new CreateDepartmentDto("Test Department", "test-department", null, null);

            using var createDepartmentStringContent = new StringContent(JsonSerializer.Serialize(createDepartmentDto), Encoding.UTF8, "application/json");

            var createDepartmentResponse = await TestHttpClient.PostAsync(new Uri("/departments/", UriKind.Relative), createDepartmentStringContent, CancellationToken.None);
            Assert.Equal(HttpStatusCode.Created, createDepartmentResponse.StatusCode);

            var createDepartmentStringResponse = await createDepartmentResponse.Content.ReadAsStringAsync(CancellationToken.None);
            Assert.NotEmpty(createDepartmentStringResponse);

            var createDepartmentEvelope = JsonSerializer.Deserialize<Envelope<Guid>>(createDepartmentStringResponse, JsonSerializerOptions.Web);
            Assert.NotNull(createDepartmentEvelope);
            Assert.NotEqual(Guid.Empty, createDepartmentEvelope.Result);
            Assert.Null(createDepartmentEvelope.Error);

            var getDepartmentResponse = await TestHttpClient.GetAsync(new Uri($"/departments/{createDepartmentEvelope.Result}", UriKind.Relative), CancellationToken.None);
            Assert.Equal(HttpStatusCode.OK, getDepartmentResponse.StatusCode);

            var getDepartmentStringResponse = await getDepartmentResponse.Content.ReadAsStringAsync(CancellationToken.None);
            Assert.NotEmpty(getDepartmentStringResponse);

            var getDepartmentEnvelope = JsonSerializer.Deserialize<Envelope<DepartmentDto>>(getDepartmentStringResponse, JsonSerializerOptions.Web);
            Assert.NotNull(getDepartmentEnvelope);
            Assert.Null(getDepartmentEnvelope.Error);
            Assert.Equal(createDepartmentEvelope.Result, getDepartmentEnvelope.Result?.Id);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CreateDepartment_with_locations_should_attach_locations(bool hasPrimaryLocation)
        {
            var firstLocationId = await CreateLocationAsync("Office A", CreateAddress("Street A"));
            var secondLocationId = await CreateLocationAsync("Office B", CreateAddress("Street B"));

            await CreateDepartmentAsync("Development", "development", null, [new(firstLocationId, hasPrimaryLocation), new(secondLocationId, false)]);

            var locations = AssertSuccess(await GetAsync<PagedResult<LocationListItemDto>>("/locations?minDepartments=1"));
            Assert.Equal(2, locations.TotalCount);
            Assert.All(locations.Items, p => Assert.Equal(1, p.DepartmentCount));
        }

        [Fact]
        public async Task CreateDepartment_with_several_primary_locations_should_return_validation_error()
        {
            var firstLocationId = await CreateLocationAsync("Office A", CreateAddress("Street A"));
            var secondLocationId = await CreateLocationAsync("Office B", CreateAddress("Street B"));

            var response = await PostAsync<Guid>("/departments/", new CreateDepartmentDto("Development", "development", null, [new(firstLocationId, true), new(secondLocationId, true)]));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, "department.locations");

            var departments = AssertSuccess(await GetAsync<PagedResult<DepartmentListItemDto>>("/departments"));
            Assert.Equal(0, departments.TotalCount);
        }

        [Fact]
        public async Task CreateDepartment_with_duplicate_locations_should_return_validation_error()
        {
            var locationId = await CreateLocationAsync("Office");

            var response = await PostAsync<Guid>("/departments/", new CreateDepartmentDto("Development", "development", null, [new(locationId, true), new(locationId, false)]));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, "department.locations");
        }

        [Fact]
        public async Task CreateDepartment_with_parent_should_build_child_path()
        {
            var parentId = await CreateDepartmentAsync("Head Office", "head");

            var childId = await CreateDepartmentAsync("Accounting", "accounting", parentId);

            var child = AssertSuccess(await GetAsync<DepartmentDto>($"/departments/{childId}"));
            Assert.Equal(parentId, child.ParentId);
            Assert.Equal("head/accounting", child.Path);
        }

        [Theory]
        [InlineData("", "valid-slug", "name.value")]
        [InlineData("A", "valid-slug", "name.value")]
        [InlineData("Valid name", "", "slug.value")]
        [InlineData("Valid name", "-invalid-", "slug.value")]
        [InlineData("Valid name", "invalid slug", "slug.value")]
        public async Task CreateDepartment_with_invalid_data_should_return_validation_error(string name, string slug, string expectedCode)
        {
            var response = await PostAsync<Guid>("/departments/", new CreateDepartmentDto(name, slug, null, null));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, expectedCode);
        }

        [Fact]
        public async Task CreateDepartment_with_not_existing_parent_should_return_not_found()
        {
            var response = await PostAsync<Guid>("/departments/", new CreateDepartmentDto("Child", "child", Guid.NewGuid(), null));

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department");
        }

        [Fact]
        public async Task CreateDepartment_with_not_existing_location_should_return_not_found()
        {
            var existingLocationId = await CreateLocationAsync("Office");

            var response = await PostAsync<Guid>("/departments/", new CreateDepartmentDto("Development", "development", null, [new(existingLocationId, true), new(Guid.NewGuid(), false)]));

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "locations");

            var departments = AssertSuccess(await GetAsync<PagedResult<DepartmentListItemDto>>("/departments"));
            Assert.Equal(0, departments.TotalCount);
        }

        #endregion

        #region GetDepartments

        [Fact]
        public async Task GetDepartments_with_search_should_return_matching_departments()
        {
            await CreateDepartmentAsync("Marketing", "marketing");
            await CreateDepartmentAsync("Market analytics", "market-analytics");
            await CreateDepartmentAsync("Support", "support");

            var departments = AssertSuccess(await GetAsync<PagedResult<DepartmentListItemDto>>("/departments?search=MARKET&sortField=name&sortDirection=ascending"));

            Assert.Equal(2, departments.TotalCount);
            Assert.Equal(["Market analytics", "Marketing"], departments.Items.Select(p => p.Name));
        }

        [Fact]
        public async Task GetDepartments_with_pagination_should_return_requested_page()
        {
            await CreateDepartmentAsync("Alpha", "alpha");
            await CreateDepartmentAsync("Beta", "beta");
            await CreateDepartmentAsync("Gamma", "gamma");

            var departments = AssertSuccess(await GetAsync<PagedResult<DepartmentListItemDto>>("/departments?sortField=name&sortDirection=Descending&page=2&pageSize=2"));

            Assert.Equal(3, departments.TotalCount);
            Assert.Equal(2, departments.PageNumber);
            Assert.Equal(2, departments.PageSize);
            Assert.Equal("Alpha", Assert.Single(departments.Items).Name);
        }

        [Theory]
        [InlineData("sortField=unknown", "departments.sortField")]
        [InlineData("sortDirection=sideways", "departments.sortDirection")]
        [InlineData("page=0", "departments.page")]
        [InlineData("pageSize=0", "departments.pageSize")]
        [InlineData("pageSize=101", "departments.pageSize")]
        public async Task GetDepartments_with_invalid_parameters_should_return_validation_error(string query, string expectedCode)
        {
            var response = await GetAsync<PagedResult<DepartmentListItemDto>>($"/departments?{query}");

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, expectedCode);
        }

        #endregion

        #region UpdateDepartment

        [Fact]
        public async Task UpdateDepartment_with_valid_data_should_update_department()
        {
            var parentId = await CreateDepartmentAsync("Head Office", "head");
            var departmentId = await CreateDepartmentAsync("Sales", "sales");

            AssertSuccess(await PatchAsync<object>($"/departments/{departmentId}", new UpdateDepartmentDto("Retail sales", "retail", parentId)));

            var department = AssertSuccess(await GetAsync<DepartmentDto>($"/departments/{departmentId}"));
            Assert.Equal("Retail sales", department.Name);
            Assert.Equal("retail", department.Slug);
            Assert.Equal("head/retail", department.Path);
            Assert.Equal(parentId, department.ParentId);
            Assert.NotNull(department.UpdatedAt);
        }

        [Fact]
        public async Task UpdateDepartment_with_not_existing_id_should_return_not_found()
        {
            var response = await PatchAsync<object>($"/departments/{Guid.NewGuid()}", new UpdateDepartmentDto("Sales", "sales", null));

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department");
        }

        [Fact]
        public async Task UpdateDepartment_with_not_existing_parent_should_return_not_found()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");

            var response = await PatchAsync<object>($"/departments/{departmentId}", new UpdateDepartmentDto("Sales", "sales", Guid.NewGuid()));

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department");
        }

        [Fact]
        public async Task UpdateDepartment_with_invalid_data_should_return_validation_error()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");

            var response = await PatchAsync<object>($"/departments/{departmentId}", new UpdateDepartmentDto("Sales", "Invalid_Slug!", null));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, "slug.value");

            var department = AssertSuccess(await GetAsync<DepartmentDto>($"/departments/{departmentId}"));
            Assert.Equal("sales", department.Slug);
        }

        [Fact]
        public async Task UpdateDepartment_with_self_as_parent_should_return_conflict()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");

            var response = await PatchAsync<object>($"/departments/{departmentId}", new UpdateDepartmentDto("Sales", "sales", departmentId));

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "department");
        }

        [Fact]
        public async Task UpdateDepartment_with_descendant_as_parent_should_return_conflict()
        {
            var rootId = await CreateDepartmentAsync("Head Office", "head");
            var childId = await CreateDepartmentAsync("Sales", "sales", rootId);

            var response = await PatchAsync<object>($"/departments/{rootId}", new UpdateDepartmentDto("Head Office", "head", childId));

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "department");
        }

        #endregion

        #region DeleteDepartment

        [Fact]
        public async Task DeleteDepartment_with_relations_should_delete_department_and_relations()
        {
            var locationId = await CreateLocationAsync("Office");
            var positionId = await CreatePositionAsync("Manager");
            var departmentId = await CreateDepartmentAsync("Sales", "sales", null, [new(locationId, true)]);
            await AttachPositionAsync(departmentId, positionId);

            AssertSuccess(await DeleteAsync<object>($"/departments/{departmentId}"));

            AssertError(await GetAsync<DepartmentDto>($"/departments/{departmentId}"), HttpStatusCode.NotFound, AppErrorType.NotFound, "department");

            // Связи удалены — локацию и позицию теперь можно удалить без конфликта
            AssertSuccess(await DeleteAsync<object>($"/locations/{locationId}"));
            AssertSuccess(await DeleteAsync<object>($"/positions/{positionId}"));
        }

        [Fact]
        public async Task DeleteDepartment_with_not_existing_id_should_return_not_found()
        {
            var response = await DeleteAsync<object>($"/departments/{Guid.NewGuid()}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department");
        }

        [Fact]
        public async Task DeleteDepartment_with_children_should_return_conflict()
        {
            var parentId = await CreateDepartmentAsync("Head Office", "head");
            await CreateDepartmentAsync("Sales", "sales", parentId);

            var response = await DeleteAsync<object>($"/departments/{parentId}");

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "department");
            AssertSuccess(await GetAsync<DepartmentDto>($"/departments/{parentId}"));
        }

        #endregion

        #region AddDepartmentLocation

        [Fact]
        public async Task AddDepartmentLocation_with_existing_entities_should_attach_location()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var locationId = await CreateLocationAsync("Office");

            AssertSuccess(await PostAsync<object>($"/departments/{departmentId}/locations/{locationId}"));

            var location = Assert.Single(AssertSuccess(await GetAsync<PagedResult<LocationListItemDto>>("/locations?minDepartments=1")).Items);
            Assert.Equal(locationId, location.Id);
        }

        [Fact]
        public async Task AddDepartmentLocation_with_not_existing_department_should_return_not_found()
        {
            var locationId = await CreateLocationAsync("Office");

            var response = await PostAsync<object>($"/departments/{Guid.NewGuid()}/locations/{locationId}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department");
        }

        [Fact]
        public async Task AddDepartmentLocation_with_not_existing_location_should_return_not_found()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");

            var response = await PostAsync<object>($"/departments/{departmentId}/locations/{Guid.NewGuid()}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "location");
        }

        [Fact]
        public async Task AddDepartmentLocation_twice_should_return_conflict()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var locationId = await CreateLocationAsync("Office");
            await AttachLocationAsync(departmentId, locationId);

            var response = await PostAsync<object>($"/departments/{departmentId}/locations/{locationId}");

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "department_location");
        }

        #endregion

        #region RemoveDepartmentLocation

        [Fact]
        public async Task RemoveDepartmentLocation_with_existing_relation_should_detach_location()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var locationId = await CreateLocationAsync("Office");
            await AttachLocationAsync(departmentId, locationId);

            AssertSuccess(await DeleteAsync<object>($"/departments/{departmentId}/locations/{locationId}"));

            var locations = AssertSuccess(await GetAsync<PagedResult<LocationListItemDto>>("/locations?minDepartments=1"));
            Assert.Empty(locations.Items);
        }

        [Fact]
        public async Task RemoveDepartmentLocation_without_relation_should_return_not_found()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var locationId = await CreateLocationAsync("Office");

            var response = await DeleteAsync<object>($"/departments/{departmentId}/locations/{locationId}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department_location");
        }

        #endregion

        #region AddDepartmentPosition

        [Fact]
        public async Task AddDepartmentPosition_with_existing_entities_should_attach_position()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var positionId = await CreatePositionAsync("Manager");

            AssertSuccess(await PostAsync<object>($"/departments/{departmentId}/positions/{positionId}"));

            // Привязанную позицию нельзя удалить — это подтверждает, что связь создана
            AssertError(await DeleteAsync<object>($"/positions/{positionId}"), HttpStatusCode.Conflict, AppErrorType.Conflict, "position");
        }

        [Fact]
        public async Task AddDepartmentPosition_with_not_existing_department_should_return_not_found()
        {
            var positionId = await CreatePositionAsync("Manager");

            var response = await PostAsync<object>($"/departments/{Guid.NewGuid()}/positions/{positionId}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department");
        }

        [Fact]
        public async Task AddDepartmentPosition_with_not_existing_position_should_return_not_found()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");

            var response = await PostAsync<object>($"/departments/{departmentId}/positions/{Guid.NewGuid()}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "position");
        }

        [Fact]
        public async Task AddDepartmentPosition_twice_should_return_conflict()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var positionId = await CreatePositionAsync("Manager");
            await AttachPositionAsync(departmentId, positionId);

            var response = await PostAsync<object>($"/departments/{departmentId}/positions/{positionId}");

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "department_position");
        }

        #endregion

        #region RemoveDepartmentPosition

        [Fact]
        public async Task RemoveDepartmentPosition_with_existing_relation_should_detach_position()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var positionId = await CreatePositionAsync("Manager");
            await AttachPositionAsync(departmentId, positionId);

            AssertSuccess(await DeleteAsync<object>($"/departments/{departmentId}/positions/{positionId}"));

            // Связь удалена — позицию можно удалить
            AssertSuccess(await DeleteAsync<object>($"/positions/{positionId}"));
        }

        [Fact]
        public async Task RemoveDepartmentPosition_without_relation_should_return_not_found()
        {
            var departmentId = await CreateDepartmentAsync("Sales", "sales");
            var positionId = await CreatePositionAsync("Manager");

            var response = await DeleteAsync<object>($"/departments/{departmentId}/positions/{positionId}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "department_position");
        }

        #endregion

        #region Arrange helpers

        private static CreateAddressDto CreateAddress(string street = "Lenina") => new("Russia", "Moscow region", "Moscow", street, "1");

        private async Task<Guid> CreateDepartmentAsync(string name, string slug, Guid? parentId = null, IEnumerable<CreateDepartmentLocationDto>? locations = null)
        {
            var response = await PostAsync<Guid>("/departments/", new CreateDepartmentDto(name, slug, parentId, locations));
            var id = AssertSuccess(response, HttpStatusCode.Created);
            Assert.NotEqual(Guid.Empty, id);

            return id;
        }

        private async Task<Guid> CreateLocationAsync(string name, CreateAddressDto? address = null)
        {
            var response = await PostAsync<Guid>("/locations/", new CreateLocationDto(name, address ?? CreateAddress()));
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

        private async Task AttachLocationAsync(Guid departmentId, Guid locationId)
        {
            AssertSuccess(await PostAsync<object>($"/departments/{departmentId}/locations/{locationId}"));
        }

        private async Task AttachPositionAsync(Guid departmentId, Guid positionId)
        {
            AssertSuccess(await PostAsync<object>($"/departments/{departmentId}/positions/{positionId}"));
        }

        #endregion
    }
}
