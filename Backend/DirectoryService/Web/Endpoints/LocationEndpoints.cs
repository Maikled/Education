using Contracts.DTOs;
using Core.Locations.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        private static async Task<IResult> GetAllAsync(CancellationToken cancellationToken)
        {
            return Results.Ok(Array.Empty<object>());
        }

        private static async Task<IResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NotFound();
        }

        private static async Task<IResult> CreateAsync([FromBody] CreateLocationDto locationDto, [FromServices] ILocationsService locationsService, CancellationToken cancellationToken)
        {
            var locationId = await locationsService.CreateLocationAsync(locationDto, cancellationToken);
            return Results.Created($"/locations/{locationId}", locationId);
        }

        private static async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateLocationDto locationDto, [FromServices] ILocationsService locationsService, CancellationToken cancellationToken)
        {
            await locationsService.UpdateLocationAsync(id, locationDto, cancellationToken);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NoContent();
        }
    }
}
