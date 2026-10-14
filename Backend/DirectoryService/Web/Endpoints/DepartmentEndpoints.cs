using Contracts.DTOs;
using Core.Departments.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
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

        private static async Task<IResult> GetAllAsync(CancellationToken cancellationToken)
        {
            return Results.Ok(Array.Empty<object>());
        }

        private static async Task<IResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NotFound();
        }

        private static async Task<IResult> CreateAsync([FromBody] CreateDepartmentDto departmentDto, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            var departmentIdResult = await departmentsService.CreateDepartmentAsync(departmentDto, cancellationToken);
            if (departmentIdResult.IsFailure)
                return departmentIdResult.Error.ToResponse();

            return Results.Created($"/departments/{departmentIdResult.Value}", departmentIdResult);
        }

        private static async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateDepartmentDto departmentDto, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            var updateDepartmentResult = await departmentsService.UpdateDepartmentAsync(id, departmentDto, cancellationToken);
            if (updateDepartmentResult.IsFailure)
                return updateDepartmentResult.Error.ToResponse();

            return Results.Ok();
        }

        private static async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NoContent();
        }

        private async Task<IResult> AddLocationToDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            var addLocationResult = await departmentsService.AddLocationsAsync(departmentId, locationId, cancellationToken);
            if (addLocationResult.IsFailure)
                return addLocationResult.Error.ToResponse();

            return Results.Ok();
        }

        private async Task<IResult> RemoveLocationFromDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            var removeLocationResult = await departmentsService.RemoveLocationsAsync(departmentId, locationId, cancellationToken);
            if (removeLocationResult.IsFailure)
                return removeLocationResult.Error.ToResponse();

            return Results.Ok();
        }
    }
}
