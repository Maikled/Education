using Contracts.DTOs;
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

        private static async Task<IResult> CreateAsync([FromBody] CreateDepartmentDto departmentDto, CancellationToken cancellationToken)
        {
            return Results.Created();
        }

        private static async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateDepartmentDto departmentDto, CancellationToken cancellationToken)
        {
            return Results.Ok();
        }

        private static async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return Results.NoContent();
        }
    }
}
