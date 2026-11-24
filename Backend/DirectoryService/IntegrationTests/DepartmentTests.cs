using Contracts.DTOs;
using Contracts.Errors.Enums;
using System.Net;
using System.Text;
using System.Text.Json;
using Web.Endpoints.Models;

namespace IntegrationTests
{
    public class DepartmentTests : DatabaseResetTestsFactory
    {
        public DepartmentTests(TestsFactory testsFactory) : base(testsFactory)
        {

        }

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
    }
}
