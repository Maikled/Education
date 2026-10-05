using Web.Endpoints;
using Web.Endpoints.Helpers;

namespace Web.Providers
{
    internal static class EndpointsProvider
    {
        public static void RegisterAppEndpoints(RouteGroupBuilder endpointsBuilder)
        {
            endpointsBuilder.Register<DepartmentEndpoints>();
            endpointsBuilder.Register<LocationEndpoints>();
            endpointsBuilder.Register<PositionEndpoints>();
        }
    }
}
