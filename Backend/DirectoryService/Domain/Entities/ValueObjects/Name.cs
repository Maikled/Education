using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Domain.Entities.ValueObjects
{
    public record Name
    {
        public string Value { get; }

        public const int MIN_LENGTH = 2;
        public const int MAX_LENGTH = 50;

        private Name(string value)
        {
            Value = value;
        }

        public static Result<Name, AppError> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return AppError.Validation("name.value", "Name cannot be null or empty.");
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length < MIN_LENGTH || normalizedValue.Length > MAX_LENGTH)
            {
                return AppError.Validation("name.value", $"Name must be between {MIN_LENGTH} and {MAX_LENGTH} characters.");
            }

            return new Name(normalizedValue);
        }
    }
}
