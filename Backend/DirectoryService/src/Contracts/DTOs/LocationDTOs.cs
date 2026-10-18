namespace Contracts.DTOs
{
    public record CreateAddressDto(string Country, string State, string City, string Street, string BuildingNumber);
    public record CreateLocationDto(string Name, CreateAddressDto Address);
    public record UpdateLocationDto(string Name, CreateAddressDto Address);
}
