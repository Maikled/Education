namespace Contracts.DTOs
{
    public record CreateDepartmentLocationDto(Guid LocationId, bool IsPrimary);
    public record CreateDepartmentDto(string Name, string Slug, Guid? ParentId, IEnumerable<CreateDepartmentLocationDto>? Locations);
    public record UpdateDepartmentDto(string Name, string Slug, Guid? ParentId);
    public record DepartmentDto(Guid Id, string Name, string Slug, string Path, Guid? ParentId, DateTime CreatedAt, DateTime? UpdatedAt);
    public record GetDepartmentsDto(string? Search, string? SortField, string SortDirection = "Ascending", int Page = 1, int PageSize = 20);
    public record DepartmentListItemDto(Guid Id, string Name, string Slug, string Path, DateTime CreatedAt);
}
