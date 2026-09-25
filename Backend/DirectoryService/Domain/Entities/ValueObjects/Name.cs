namespace Domain.Entities.ValueObjects
{
    public record Name
    {
        public string Value { get; }

        private const int _MIN_LENGHT = 2;
        private const int _MAX_LENGHT = 50;

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

            if (normalizedValue.Length < _MIN_LENGHT || normalizedValue.Length > _MAX_LENGHT)
            {
                throw new ArgumentException($"Name must be between {_MIN_LENGHT} and {_MAX_LENGHT} characters.", nameof(value));
            }

            return new Name(normalizedValue);
        }
    }
}
