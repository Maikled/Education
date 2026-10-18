using Web.Endpoints.Models;

namespace Web.Endpoints.Helpers
{
    internal static class EndpointMetadata
    {
        private static readonly string[] Json = ["application/json"];

        public static void AddSuccess<T>(EndpointBuilder builder, int statusCode) =>
            builder.Metadata.Add(new ProducesResponseTypeMetadata(statusCode, typeof(Envelope<T>), Json));

        public static void AddErrors(EndpointBuilder builder)
        {
            int[] codes = [StatusCodes.Status400BadRequest, StatusCodes.Status404NotFound,
                       StatusCodes.Status409Conflict, StatusCodes.Status500InternalServerError];
            
            foreach (var code in codes)
                builder.Metadata.Add(new ProducesResponseTypeMetadata(code, typeof(Envelope<object>), Json));
        }
    }
}
