using Contracts.Errors;
using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace Domain.Entities.ValueObjects
{
    public partial record Slug
    {
        public string Value { get; }
        public const int MIN_LENGTH = 2;
        public const int MAX_LENGTH = 100;

        [GeneratedRegex(_REGEX_PATTERN, RegexOptions.CultureInvariant, 100)]
        private static partial Regex SlugPattern { get; }

        private const string _REGEX_PATTERN = @"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

        private Slug(string value)
        {
            Value = value;
        }

        public static Result<Slug, AppError> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return AppError.Validation("slug.value", "Slug cannot be null or empty.");
            }

            var normalizedValue = value.Trim().ToLowerInvariant();

            if (normalizedValue.Length < MIN_LENGTH || normalizedValue.Length > MAX_LENGTH)
            {
                return AppError.Validation("slug.value", $"Slug must be between {MIN_LENGTH} and {MAX_LENGTH} characters long.");
            }

            if (!SlugPattern.IsMatch(normalizedValue))
            {
                return AppError.Validation("slug.value", "Slug must only contain lowercase letters, numbers, and hyphens, and cannot start or end with a hyphen.");
            }

            return new Slug(normalizedValue);
        }
    }
}
