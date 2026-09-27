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

        public static Name Create(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value), "Name cannot be null or empty.");
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length < MIN_LENGTH || normalizedValue.Length > MAX_LENGTH)
            {
                throw new ArgumentException($"Name must be between {MIN_LENGTH} and {MAX_LENGTH} characters.", nameof(value));
            }

            return new Name(normalizedValue);
        }
    }
}
