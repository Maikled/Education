using Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;

namespace Web.Endpoints.Helpers
{
    internal sealed class PositionEndpoints : IEndpoint
    {
        public void Register(IEndpointRouteBuilder endpointsBuilder)
        {
            var group = endpointsBuilder.MapGroup("/positions");

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

        private static async Task<IResult> CreateAsync([FromBody] CreatePositionDto positionDto, CancellationToken cancellationToken)
        {
            return Results.Created();
        }

        private static async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdatePositionDto positionDto, CancellationToken cancellationToken)
        {
            return Results.Ok();
        }

        private static async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NoContent();
        }
    }
}
