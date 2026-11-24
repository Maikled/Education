using Contracts.Errors.Enums;
using System.Net;
using System.Text;
using System.Text.Json;
using Web.Endpoints.Models;

namespace IntegrationTests
{
    public sealed record ApiResponse<TValue>(HttpStatusCode StatusCode, Envelope<TValue>? Envelope);

    /// <summary>
    /// Общие хелперы для интеграционных тестов API: отправка запросов, проверки ответов и подготовка данных.
    /// </summary>
    public abstract class ApiTestsBase : DatabaseResetTestsFactory
    {
        protected ApiTestsBase(TestsFactory testsFactory) : base(testsFactory)
        {
        }

        #region HTTP helpers

        protected Task<ApiResponse<TValue>> GetAsync<TValue>(string url) => SendAsync<TValue>(HttpMethod.Get, url, null);

        protected Task<ApiResponse<TValue>> PostAsync<TValue>(string url, object? body = null) => SendAsync<TValue>(HttpMethod.Post, url, body);

        protected Task<ApiResponse<TValue>> PatchAsync<TValue>(string url, object body) => SendAsync<TValue>(HttpMethod.Patch, url, body);

        protected Task<ApiResponse<TValue>> DeleteAsync<TValue>(string url) => SendAsync<TValue>(HttpMethod.Delete, url, null);

        private async Task<ApiResponse<TValue>> SendAsync<TValue>(HttpMethod method, string url, object? body)
        {
            using var request = new HttpRequestMessage(method, new Uri(url, UriKind.Relative));
            if (body != null)
                request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            using var response = await TestHttpClient.SendAsync(request, CancellationToken.None);
            var content = await response.Content.ReadAsStringAsync(CancellationToken.None);

            if (string.IsNullOrWhiteSpace(content))
                return new ApiResponse<TValue>(response.StatusCode, null);

            // В ответе с ошибкой result = null, что не десериализуется в value-типы (например Guid)
            if (!response.IsSuccessStatusCode)
            {
                var errorEnvelope = JsonSerializer.Deserialize<Envelope<object>>(content, JsonSerializerOptions.Web);
                return new ApiResponse<TValue>(response.StatusCode, errorEnvelope == null ? null : new Envelope<TValue>(default, errorEnvelope.Error));
            }

            return new ApiResponse<TValue>(response.StatusCode, JsonSerializer.Deserialize<Envelope<TValue>>(content, JsonSerializerOptions.Web));
        }

        #endregion

        #region Assert helpers

        protected static TValue AssertSuccess<TValue>(ApiResponse<TValue> response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Envelope);
            Assert.Null(response.Envelope.Error);
            Assert.NotNull(response.Envelope.Result);

            return response.Envelope.Result;
        }

        protected static void AssertSuccess(ApiResponse<object> response)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Envelope);
            Assert.Null(response.Envelope.Error);
        }

        protected static void AssertError<TValue>(ApiResponse<TValue> response, HttpStatusCode expectedStatusCode, AppErrorType expectedType, string expectedCode)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.NotNull(response.Envelope);
            Assert.NotNull(response.Envelope.Error);
            Assert.Equal(expectedType, response.Envelope.Error.Type);
            Assert.Equal(expectedCode, response.Envelope.Error.Code);
        }

        #endregion
    }
}
