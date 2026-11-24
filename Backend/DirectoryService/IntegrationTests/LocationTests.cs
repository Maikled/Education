using Contracts;
using Contracts.DTOs;
using Contracts.Errors.Enums;
using System.Net;

namespace IntegrationTests
{
    [Collection(IntegrationTestsCollection.Name)]
    public class LocationTests : ApiTestsBase
    {
        public LocationTests(TestsFactory testsFactory) : base(testsFactory)
        {

        }

        #region CreateLocation

        [Fact]
        public async Task CreateLocation_with_valid_data_should_return_created_location()
        {
            var address = new CreateAddressDto(" Russia ", "Moscow region", "Moscow", "Tverskaya", "7");

            var locationId = await CreateLocationAsync("  Main office  ", address);

            var location = AssertSuccess(await GetAsync<LocationDto>($"/locations/{locationId}"));
            Assert.Equal(locationId, location.Id);
            Assert.Equal("Main office", location.Name);
            Assert.Equal(new AddressDto("Russia", "Moscow region", "Moscow", "Tverskaya", "7"), location.Address);
            Assert.Null(location.UpdatedAt);
        }

        [Theory]
        [InlineData("", "Lenina", "name.value")]
        [InlineData("A", "Lenina", "name.value")]
        [InlineData("Main office", "", "address.street")]
        public async Task CreateLocation_with_invalid_data_should_return_validation_error(string name, string street, string expectedCode)
        {
            var response = await PostAsync<Guid>("/locations/", new CreateLocationDto(name, CreateAddress(street)));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, expectedCode);
        }

        [Fact]
        public async Task CreateLocation_with_existing_name_should_return_conflict()
        {
            await CreateLocationAsync("Main office");

            var response = await PostAsync<Guid>("/locations/", new CreateLocationDto("Main office", CreateAddress("Other street")));

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "location.name");
        }

        #endregion

        #region GetLocation

        [Fact]
        public async Task GetLocation_with_not_existing_id_should_return_not_found()
        {
            var response = await GetAsync<LocationDto>($"/locations/{Guid.NewGuid()}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "location");
        }

        #endregion

        #region GetLocations

        [Fact]
        public async Task GetLocations_with_filters_should_return_matching_locations()
        {
            var firstOfficeId = await CreateLocationAsync("Office North", CreateAddress("Street A"));
            var secondOfficeId = await CreateLocationAsync("Office South", CreateAddress("Street B"));
            await CreateLocationAsync("Warehouse", CreateAddress("Street C"));

            var salesId = await CreateDepartmentAsync("Sales", "sales");
            var supportId = await CreateDepartmentAsync("Support", "support");
            await AttachLocationAsync(salesId, firstOfficeId);
            await AttachLocationAsync(salesId, secondOfficeId);
            await AttachLocationAsync(supportId, secondOfficeId);

            var locations = AssertSuccess(await GetAsync<PagedResult<LocationListItemDto>>("/locations?search=office&minDepartments=1&sortField=departmentsCount&sortDirection=Descending"));

            Assert.Equal(2, locations.TotalCount);
            Assert.Equal([secondOfficeId, firstOfficeId], locations.Items.Select(p => p.Id));
            Assert.Equal([2, 1], locations.Items.Select(p => p.DepartmentCount));
        }

        [Fact]
        public async Task GetLocations_with_pagination_should_return_requested_page()
        {
            await CreateLocationAsync("Alpha", CreateAddress("Street A"));
            await CreateLocationAsync("Beta", CreateAddress("Street B"));
            await CreateLocationAsync("Gamma", CreateAddress("Street C"));

            var locations = AssertSuccess(await GetAsync<PagedResult<LocationListItemDto>>("/locations?sortField=name&page=2&pageSize=2"));

            Assert.Equal(3, locations.TotalCount);
            Assert.Equal(2, locations.PageNumber);
            Assert.Equal(2, locations.PageSize);
            Assert.Equal("Gamma", Assert.Single(locations.Items).Name);
        }

        [Theory]
        [InlineData("sortField=unknown", "locations.sortField")]
        [InlineData("sortDirection=sideways", "locations.sortDirection")]
        [InlineData("minDepartments=-1", "locations.minDepartments")]
        [InlineData("page=0", "locations.page")]
        [InlineData("pageSize=0", "locations.pageSize")]
        [InlineData("pageSize=101", "locations.pageSize")]
        public async Task GetLocations_with_invalid_parameters_should_return_validation_error(string query, string expectedCode)
        {
            var response = await GetAsync<PagedResult<LocationListItemDto>>($"/locations?{query}");

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, expectedCode);
        }

        #endregion

        #region GetTopLocations

        [Fact]
        public async Task GetTopLocations_should_return_locations_ordered_by_department_count()
        {
            var locationIds = new List<Guid>();
            for (var i = 0; i < 6; i++)
            {
                locationIds.Add(await CreateLocationAsync($"Office {i}", CreateAddress($"Street {i}")));
            }

            var salesId = await CreateDepartmentAsync("Sales", "sales");
            var supportId = await CreateDepartmentAsync("Support", "support");
            await AttachLocationAsync(salesId, locationIds[5]);
            await AttachLocationAsync(salesId, locationIds[4]);
            await AttachLocationAsync(supportId, locationIds[5]);

            var topLocations = AssertSuccess(await GetAsync<List<TopLocationDto>>("/locations/top"));

            Assert.Equal(5, topLocations.Count);
            Assert.Equal(locationIds[5], topLocations[0].Id);
            Assert.Equal(2, topLocations[0].DepartmentCount);
            Assert.Equal(locationIds[4], topLocations[1].Id);
            Assert.Equal(1, topLocations[1].DepartmentCount);
            Assert.All(topLocations.Skip(2), p => Assert.Equal(0, p.DepartmentCount));
        }

        [Fact]
        public async Task GetTopLocations_without_locations_should_return_empty_list()
        {
            var topLocations = AssertSuccess(await GetAsync<List<TopLocationDto>>("/locations/top"));

            Assert.Empty(topLocations);
        }

        #endregion

        #region UpdateLocation

        [Fact]
        public async Task UpdateLocation_with_valid_data_should_update_location()
        {
            var locationId = await CreateLocationAsync("Main office");
            var newAddress = new CreateAddressDto("Russia", "Leningrad region", "Saint Petersburg", "Nevsky", "28");

            AssertSuccess(await PatchAsync<object>($"/locations/{locationId}", new UpdateLocationDto("Head office", newAddress)));

            var location = AssertSuccess(await GetAsync<LocationDto>($"/locations/{locationId}"));
            Assert.Equal("Head office", location.Name);
            Assert.Equal(new AddressDto("Russia", "Leningrad region", "Saint Petersburg", "Nevsky", "28"), location.Address);
            Assert.NotNull(location.UpdatedAt);
        }

        [Fact]
        public async Task UpdateLocation_with_not_existing_id_should_return_not_found()
        {
            var response = await PatchAsync<object>($"/locations/{Guid.NewGuid()}", new UpdateLocationDto("Head office", CreateAddress()));

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "location.not_found");
        }

        [Fact]
        public async Task UpdateLocation_with_invalid_data_should_return_validation_error()
        {
            var locationId = await CreateLocationAsync("Main office");

            var response = await PatchAsync<object>($"/locations/{locationId}", new UpdateLocationDto("", CreateAddress()));

            AssertError(response, HttpStatusCode.BadRequest, AppErrorType.Validation, "name.value");

            var location = AssertSuccess(await GetAsync<LocationDto>($"/locations/{locationId}"));
            Assert.Equal("Main office", location.Name);
        }

        #endregion

        #region DeleteLocation

        [Fact]
        public async Task DeleteLocation_with_existing_id_should_delete_location()
        {
            var locationId = await CreateLocationAsync("Main office");

            AssertSuccess(await DeleteAsync<object>($"/locations/{locationId}"));

            AssertError(await GetAsync<LocationDto>($"/locations/{locationId}"), HttpStatusCode.NotFound, AppErrorType.NotFound, "location");
        }

        [Fact]
        public async Task DeleteLocation_with_not_existing_id_should_return_not_found()
        {
            var response = await DeleteAsync<object>($"/locations/{Guid.NewGuid()}");

            AssertError(response, HttpStatusCode.NotFound, AppErrorType.NotFound, "location");
        }

        [Fact]
        public async Task DeleteLocation_attached_to_department_should_return_conflict()
        {
            var locationId = await CreateLocationAsync("Main office");
            await CreateDepartmentAsync("Sales", "sales", null, [new(locationId, true)]);

            var response = await DeleteAsync<object>($"/locations/{locationId}");

            AssertError(response, HttpStatusCode.Conflict, AppErrorType.Conflict, "location");
            AssertSuccess(await GetAsync<LocationDto>($"/locations/{locationId}"));
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

        private async Task AttachLocationAsync(Guid departmentId, Guid locationId)
        {
            AssertSuccess(await PostAsync<object>($"/departments/{departmentId}/locations/{locationId}"));
        }

        #endregion
    }
}
