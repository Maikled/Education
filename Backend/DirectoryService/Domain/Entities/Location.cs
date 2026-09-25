using Domain.Entities.ValueObjects;

namespace Domain.Entities
{
    public class Location
    {
        public Guid Id { get; private set; }
        public Name Name { get; private set; }
        public Address Address { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Location(Name name, Address address)
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
