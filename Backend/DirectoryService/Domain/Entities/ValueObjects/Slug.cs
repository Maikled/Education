using System.Text.RegularExpressions;

namespace Domain.Entities.ValueObjects
{
    public partial record Slug
    {
        public string Value { get; }

        [GeneratedRegex(_REGEX_PATTERN, RegexOptions.CultureInvariant, 100)]
        private static partial Regex SlugPattern { get; }
        private const int _MIN_LENGTH = 2;
        private const int _MAX_LENGTH = 100;
        private const string _REGEX_PATTERN = @"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

        private Slug(string value)
        {
            Value = value;
        }

        public static Slug Create(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value), "Slug cannot be null or empty.");
            }

            var normalizedValue = value.Trim().ToLowerInvariant();

            if (normalizedValue.Length < _MIN_LENGTH || normalizedValue.Length > _MAX_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Slug must be between {_MIN_LENGTH} and {_MAX_LENGTH} characters long.");
            }

            if (!SlugPattern.IsMatch(normalizedValue))
            {
                throw new ArgumentException("Slug must only contain lowercase letters, numbers, and hyphens, and cannot start or end with a hyphen.", nameof(value));
            }

            return new Slug(normalizedValue);
        }
    }
}
