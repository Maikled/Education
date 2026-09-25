using Domain.Entities.ValueObjects;

namespace Domain.Entities
{
    public class Position
    {
        public Guid Id { get; private set; }
        public Name Name { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Position(Name name)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            CreatedAt = DateTime.UtcNow;
        }

        public static Position Create(Name name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), "Position name cannot be null.");
            }

            return new Position(name);
        }
    }
}
