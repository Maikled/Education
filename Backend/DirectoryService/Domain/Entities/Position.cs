using Contracts.Errors;
using CSharpFunctionalExtensions;
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

        public static Result<Position, AppError> Create(Name name)
        {
            if (name == null)
            {
                return AppError.Validation("position.name", "Position name cannot be null.");
            }

            return new Position(name);
        }
    }
}
