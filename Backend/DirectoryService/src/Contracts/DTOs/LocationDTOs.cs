namespace Contracts.DTOs
{
    public record CreateAddressDto(string Country, string State, string City, string Street, string BuildingNumber);
    public record CreateLocationDto(string Name, CreateAddressDto Address);
    public record UpdateLocationDto(string Name, CreateAddressDto Address);
    public record AddressDto(string Country, string State, string City, string Street, string BuildingNumber);
    public record LocationDto(Guid Id, string Name, AddressDto Address, DateTime CreatedAt, DateTime? UpdatedAt);
    public record TopLocationDto(Guid Id, string Name, AddressDto Address, int DepartmentCount);
    public record GetLocationsDto(string? Search, int? MinDepartments, string? SortField, string SortDirection = "Ascending", int Page = 1, int PageSize = 20);
    public record LocationListItemDto(Guid Id, string Name, AddressDto Address, DateTime CreatedAt, int DepartmentCount);
}
