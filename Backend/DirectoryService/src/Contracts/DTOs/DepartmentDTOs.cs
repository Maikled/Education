namespace Contracts.DTOs
{
    public record CreateDepartmentDto(string Name, string Slug, Guid? ParentId, IEnumerable<Guid>? locationsIds);
    public record UpdateDepartmentDto(string Name, string Slug, Guid? ParentId);
    public record DepartmentDto(Guid Id, string Name, string Slug, string Path, Guid? ParentId, DateTime CreatedAt, DateTime? UpdatedAt);
}
