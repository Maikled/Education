using Domain.Entities.ValueObjects;

namespace Domain.Entities
{
    public class Location
    {
        public Guid Id { get; private set; }
        public Name Name { get; private set; } = null!;
        public Address Address { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Location() { } // Required by EF Core

        private Location(Name name, Address address) : this()
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Address = address;
            CreatedAt = DateTime.UtcNow;
        }

        public static Location Create(Name name, Address address)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), "Location name cannot be null.");
            }

            if (address == null)
            {
                throw new ArgumentNullException(nameof(address), "Location address cannot be null.");
            }

            return new Location(name, address);
        }
    }
}
