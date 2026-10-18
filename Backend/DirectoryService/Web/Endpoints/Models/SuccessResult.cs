namespace Web.Endpoints.Models
{
    internal class SuccessResult<TValue> : IResult
    {
        private readonly int _statusCode;
        private readonly TValue _value;

        public SuccessResult(int statusCode, TValue value)
        {
            _statusCode = statusCode;
            _value = value;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            httpContext.Response.StatusCode = _statusCode;

            return httpContext.Response.WriteAsJsonAsync(Envelope.Ok(_value));
        }
    }
}
