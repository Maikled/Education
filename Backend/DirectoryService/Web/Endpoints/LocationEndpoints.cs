using Contracts.DTOs;
using Core.Locations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
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
            var createLocationResult = await locationsService.CreateLocationAsync(locationDto, cancellationToken);
            if (createLocationResult.IsFailure)
                return createLocationResult.Error.ToResponse();

            return Results.Created($"/locations/{createLocationResult.Value}", createLocationResult.Value);
        }

        private static async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateLocationDto locationDto, [FromServices] ILocationsService locationsService, CancellationToken cancellationToken)
        {
            var updateLocationResult = await locationsService.UpdateLocationAsync(id, locationDto, cancellationToken);
            if (updateLocationResult.IsFailure)
                return updateLocationResult.Error.ToResponse();

            return Results.Ok();
        }

        private static async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NoContent();
        }
    }
}
