using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Domain.Entities.ValueObjects
{
    public record Address
    {
        public const int MAX_LENGTH = 500;

        public string Country { get; }
        public string State { get; }
        public string City { get; }
        public string Street { get; }
        public string BuildingNumber { get; }

        private Address(string country, string state, string city, string street, string buildingNumber)
        {
            Country = country;
            State = state;
            City = city;
            Street = street;
            BuildingNumber = buildingNumber;
        }

        public static Result<Address, AppError> Create(string country, string state, string city, string street, string buildingNumber)
        {
            var validationResult = Validate(country, state, city, street, buildingNumber);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Address>();

            return new Address(Normalize(country), Normalize(state), Normalize(city), Normalize(street), Normalize(buildingNumber));
        }

        public static UnitResult<AppError> Validate(string country, string state, string city, string street, string buildingNumber)
        {
            return Result.Combine(
                errors => AppError.FromErrors(errors),
                ValidateParameter(country, nameof(country)),
                ValidateParameter(state, nameof(state)),
                ValidateParameter(city, nameof(city)),
                ValidateParameter(street, nameof(street)),
                ValidateParameter(buildingNumber, nameof(buildingNumber)));
        }

        private static string Normalize(string parameter) => parameter.Trim();

        private static UnitResult<AppError> ValidateParameter(string parameter, string parameterName)
        {
            var notEmptyResult = ValidateNotEmpty(parameter, parameterName);
            if (notEmptyResult.IsFailure)
                return notEmptyResult;

            return ValidateLength(Normalize(parameter), parameterName);
        }

        private static UnitResult<AppError> ValidateNotEmpty(string parameter, string parameterName)
        {
            return UnitResult.FailureIf(string.IsNullOrEmpty(parameter), AppError.Validation($"address.{parameterName}", $"{parameterName} cannot be null or empty."));
        }

        private static UnitResult<AppError> ValidateLength(string normalizedParameter, string parameterName)
        {
            return UnitResult.FailureIf(normalizedParameter.Length > MAX_LENGTH, AppError.Validation($"address.{parameterName}", $"{parameterName} exceeds the maximum length of {MAX_LENGTH} characters."));
        }
    }
}
