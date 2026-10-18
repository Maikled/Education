using Contracts.DTOs;
using Core.Departments.Interfaces;
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

        private static async Task<CreatedEndpointResult<Guid>> CreateAsync([FromBody] CreateDepartmentDto departmentDto, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            return await departmentsService.CreateDepartmentAsync(departmentDto, cancellationToken);
        }

        private static async Task<EndpointResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateDepartmentDto departmentDto, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            return await departmentsService.UpdateDepartmentAsync(id, departmentDto, cancellationToken);
        }

        private static async Task<EndpointResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return EndpointResult.Empty;
        }

        private async Task<EndpointResult> AddLocationToDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            return await departmentsService.AddLocationsAsync(departmentId, locationId, cancellationToken);
        }

        private async Task<EndpointResult> RemoveLocationFromDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            return await departmentsService.RemoveLocationsAsync(departmentId, locationId, cancellationToken);
        }
    }
}
