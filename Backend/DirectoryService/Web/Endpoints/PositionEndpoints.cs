using Contracts.DTOs;
using Core.Interfaces;
using Core.Positions.CreatePosition;
using Core.Positions.DeletePosition;
using Core.Positions.UpdatePosition;
using Microsoft.AspNetCore.Mvc;
using Web.Endpoints.Models;
using Web.Interfaces;

namespace Web.Endpoints
{
    internal sealed class PositionEndpoints : IEndpoint
    {
        public void Register(IEndpointRouteBuilder endpointsBuilder)
        {
            var group = endpointsBuilder.MapGroup("/positions");

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

        private static async Task<CreatedEndpointResult<Guid>> CreateAsync([FromBody] CreatePositionDto positionDto, [FromServices] ICommandHandler<Guid, CreatePositionCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new CreatePositionCommand(positionDto), cancellationToken);
        }

        private static async Task<EndpointResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdatePositionDto positionDto, [FromServices] ICommandHandler<UpdatePositionCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new UpdatePositionCommand(id, positionDto), cancellationToken);
        }

        private static async Task<EndpointResult> DeleteAsync([FromRoute] Guid id, [FromServices] ICommandHandler<DeletePositionCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new DeletePositionCommand(id), cancellationToken);
        }
    }
}
