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
            var validationResult = Validate(value);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Name>();

            return new Name(Normalize(value));
        }

        public static UnitResult<AppError> Validate(string value)
        {
            var notEmptyResult = ValidateNotEmpty(value);
            if (notEmptyResult.IsFailure)
                return notEmptyResult;

            return ValidateLength(Normalize(value));
        }

        private static string Normalize(string value) => value.Trim();

        private static UnitResult<AppError> ValidateNotEmpty(string value)
        {
            return UnitResult.FailureIf(string.IsNullOrEmpty(value), AppError.Validation("name.value", "Name cannot be null or empty."));
        }

        private static UnitResult<AppError> ValidateLength(string normalizedValue)
        {
            return UnitResult.FailureIf(normalizedValue.Length < MIN_LENGTH || normalizedValue.Length > MAX_LENGTH, AppError.Validation("name.value", $"Name must be between {MIN_LENGTH} and {MAX_LENGTH} characters."));
        }
    }
}
