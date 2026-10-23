using Contracts.DTOs;
using Core.Interfaces;
using Core.Locations.CreateLocation;
using Core.Locations.UpdateLocation;
using Microsoft.AspNetCore.Mvc;
using Web.Endpoints.Models;
using Web.Interfaces;

namespace Web.Endpoints
{
    internal sealed class LocationEndpoints : IEndpoint
    {
        public void Register(IEndpointRouteBuilder endpointsBuilder)
        {
            var group = endpointsBuilder.MapGroup("/locations");

            group.MapPost("/", CreateAsync);
            group.MapGet("/", GetAllAsync);
            group.MapGet("/{id:guid}", GetByIdAsync);
            group.MapPatch("/{id:guid}", UpdateAsync);
            group.MapDelete("/{id:guid}", DeleteAsync);
        }

        private static async Task<EndpointResult> GetAllAsync(CancellationToken cancellationToken)
        {
            return EndpointResult.Empty;
        }

        private static async Task<EndpointResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return EndpointResult.Empty;
        }

        private static async Task<CreatedEndpointResult<Guid>> CreateAsync([FromBody] CreateLocationDto locationDto, [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new CreateLocationCommand(locationDto), cancellationToken);
        }

        private static async Task<EndpointResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateLocationDto locationDto, [FromServices] ICommandHandler<UpdateLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new UpdateLocationCommand(id, locationDto), cancellationToken);
        }

        private static async Task<EndpointResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return EndpointResult.Empty;
        }
    }
}
