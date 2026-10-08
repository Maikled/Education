using Contracts.DTOs;
using Core.Departments.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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
            try
            {
                var departmentId = await departmentsService.CreateDepartmentAsync(departmentDto, cancellationToken);
                return Results.Created($"/departments/{departmentId}", departmentId);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateDepartmentDto departmentDto, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            try
            {
                await departmentsService.UpdateDepartmentAsync(id, departmentDto, cancellationToken);
                return Results.Ok();
            }
            catch(Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NoContent();
        }

        private async Task<IResult> AddLocationToDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            try
            {
                await departmentsService.AddLocationsAsync(departmentId, locationId, cancellationToken);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private async Task<IResult> RemoveLocationFromDepartmentAsync([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromServices] IDepartmentService departmentsService, CancellationToken cancellationToken)
        {
            try
            {
                await departmentsService.RemoveLocationsAsync(departmentId, locationId, cancellationToken);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
