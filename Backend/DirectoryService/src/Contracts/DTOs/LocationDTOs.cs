namespace Contracts.DTOs
{
    public record CreateLocationDto(string Name, string Country, string State, string City, string Street, string BuildingNumber);
    public record UpdateLocationDto(string Name, string Country, string State, string City, string Street, string BuildingNumber);
}
