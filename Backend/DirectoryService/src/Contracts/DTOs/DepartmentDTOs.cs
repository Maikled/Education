namespace Contracts.DTOs
{
    public record CreateDepartmentDto(string Name, string Slug, Guid? ParentId, IEnumerable<Guid> locationsIds);
    public record UpdateDepartmentDto(string Name, string Slug, Guid? ParentId);
}
