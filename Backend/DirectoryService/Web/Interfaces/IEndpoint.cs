namespace Web.Interfaces
{
    internal interface IEndpoint
    {
        void Register(IEndpointRouteBuilder endpointsBuilder);
    }
}