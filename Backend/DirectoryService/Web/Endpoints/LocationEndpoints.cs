using Contracts.DTOs;
using Core.Locations.Interfaces;
using FluentValidation;
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
            group.MapPut("/{id:guid}", UpdateAsync);
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
            try
            {
                var locationId = await locationsService.CreateLocationAsync(locationDto, cancellationToken);
                return Results.Created($"/locations/{locationId}", locationId);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ex.Errors);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateLocationDto locationDto, CancellationToken cancellationToken)
        {
            return Results.Ok();
        }

        private static async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NoContent();
        }
    }
}
