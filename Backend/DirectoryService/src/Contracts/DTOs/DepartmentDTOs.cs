namespace Contracts.DTOs
{
    public record CreateDepartmentDto(string Name, string Slug, Guid? ParentId);
    public record UpdateDepartmentDto(string Name, string Slug, Guid? ParentId);
}
