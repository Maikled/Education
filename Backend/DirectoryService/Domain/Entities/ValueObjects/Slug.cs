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
            var validationResult = Validate(value);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Slug>();

            return new Slug(Normalize(value));
        }

        public static UnitResult<AppError> Validate(string value)
        {
            var notEmptyResult = ValidateNotEmpty(value);
            if (notEmptyResult.IsFailure)
                return notEmptyResult;

            var normalizedValue = Normalize(value);

            return Result.Combine(errors => AppError.FromErrors(errors), ValidateLength(normalizedValue), ValidateFormat(normalizedValue));
        }

        private static string Normalize(string value) => value.Trim().ToLowerInvariant();

        private static UnitResult<AppError> ValidateNotEmpty(string value)
        {
            return UnitResult.FailureIf(string.IsNullOrEmpty(value), AppError.Validation("slug.value", "Slug cannot be null or empty."));
        }

        private static UnitResult<AppError> ValidateLength(string normalizedValue)
        {
            return UnitResult.FailureIf(normalizedValue.Length < MIN_LENGTH || normalizedValue.Length > MAX_LENGTH, AppError.Validation("slug.value", $"Slug must be between {MIN_LENGTH} and {MAX_LENGTH} characters long."));
        }

        private static UnitResult<AppError> ValidateFormat(string normalizedValue)
        {
            return UnitResult.FailureIf(!SlugPattern.IsMatch(normalizedValue), AppError.Validation("slug.value", "Slug must only contain lowercase letters, numbers, and hyphens, and cannot start or end with a hyphen."));
        }
    }
}
