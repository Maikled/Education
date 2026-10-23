using Contracts.DTOs;
using Core.Departments.AddDepartmentLocation;
using Core.Departments.CreateDepartment;
using Core.Departments.RemoveDepartmentLocation;
using Core.Departments.UpdateDepartment;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Endpoints.Models;
using Web.Interfaces;

namespace Web.Endpoints
{
    internal sealed class DepartmentEndpoints : IEndpoint
    {
        public void Register(IEndpointRouteBuilder endpointsBuilder)
        {
            var group = endpointsBuilder.MapGroup("/departments");

            group.MapPost("/", CreateAsync);
            group.MapGet("/", GetAllAsync);
            group.MapGet("/{id:guid}", GetByIdAsync);
            group.MapPatch("/{id:guid}", UpdateAsync);
            group.MapDelete("/{id:guid}", DeleteAsync);
            group.MapPost("/{departmentId:guid}/locations/{locationId:guid}", AddLocationToDepartmentAsync);
            group.MapDelete("/{departmentId:guid}/locations/{locationId:guid}", RemoveLocationFromDepartmentAsync);
        }

        private static async Task<EndpointResult> GetAllAsync(CancellationToken cancellationToken)
        {
            return EndpointResult.Empty;
        }

        private static async Task<EndpointResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return EndpointResult.Empty;
        }

        private static async Task<CreatedEndpointResult<Guid>> CreateAsync([FromBody] CreateDepartmentDto departmentDto, [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new CreateDepartmentCommand(departmentDto), cancellationToken);
        }

        private static async Task<EndpointResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateDepartmentDto departmentDto, [FromServices] ICommandHandler<UpdateDepartmentCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new UpdateDepartmentCommand(departmentDto, id), cancellationToken);
        }

        private static async Task<EndpointResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return EndpointResult.Empty;
        }

        private async Task<EndpointResult> AddLocationToDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] ICommandHandler<AddDepartmentLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new AddDepartmentLocationCommand(departmentId, locationId), cancellationToken);
        }

        private async Task<EndpointResult> RemoveLocationFromDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] ICommandHandler<RemoveDepartmentLocationCommand> handler, CancellationToken cancellationToken)
        {
            return await handler.HandlerAsync(new RemoveDepartmentLocationCommand(departmentId, locationId), cancellationToken);
        }
    }
}
