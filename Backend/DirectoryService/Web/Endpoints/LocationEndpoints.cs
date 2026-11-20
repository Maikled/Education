using Contracts;
using Contracts.DTOs;
using Core.Interfaces;
using Core.Locations.CreateLocation;
using Core.Locations.DeleteLocation;
using Core.Locations.GetLocation;
using Core.Locations.GetLocations;
using Core.Locations.TopLocations;
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
            group.MapGet("/top", GetTopAsync);
            group.MapPatch("/{id:guid}", UpdateAsync);
            group.MapDelete("/{id:guid}", DeleteAsync);
        }

        private static async Task<EndpointResult<PagedResult<LocationListItemDto>>> GetAllAsync([AsParameters] GetLocationsDto getLocationsDto, [FromServices] ICommandHandler<PagedResult<LocationListItemDto>, GetLocationsCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new GetLocationsCommand(getLocationsDto.Search, getLocationsDto.MinDepartments, getLocationsDto.SortField, getLocationsDto.SortDirection, getLocationsDto.Page, getLocationsDto.PageSize), cancellationToken);
        }

        private static async Task<EndpointResult<LocationDto>> GetByIdAsync([FromRoute] Guid id, [FromServices] ICommandHandler<LocationDto, GetLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new GetLocationCommand(id), cancellationToken);
        }

        private static async Task<EndpointResult<IEnumerable<TopLocationDto>>> GetTopAsync([FromServices] ICommandHandler<IEnumerable<TopLocationDto>, GetTopLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new GetTopLocationCommand(5), cancellationToken);
        }

        private static async Task<CreatedEndpointResult<Guid>> CreateAsync([FromBody] CreateLocationDto locationDto, [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new CreateLocationCommand(locationDto), cancellationToken);
        }

        private static async Task<EndpointResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateLocationDto locationDto, [FromServices] ICommandHandler<UpdateLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new UpdateLocationCommand(id, locationDto), cancellationToken);
        }

        private static async Task<EndpointResult> DeleteAsync([FromRoute] Guid id, [FromServices] ICommandHandler<DeleteLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new DeleteLocationCommand(id), cancellationToken);
        }
    }
}
