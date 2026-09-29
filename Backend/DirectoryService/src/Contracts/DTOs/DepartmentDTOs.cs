namespace Contracts.DTOs
{
    public record CreateDepartmentDto(string Name, string Slug, Guid? parentId);
    public record UpdateDepartmentDto(string Name, string Slug, Guid? parentId);
}
