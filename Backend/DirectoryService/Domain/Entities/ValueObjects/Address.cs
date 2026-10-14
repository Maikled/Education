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
            var countryResult = ValidateParameter(country, nameof(country));
            if (countryResult.IsFailure)
            {
                return countryResult.Error;
            }

            var stateResult = ValidateParameter(state, nameof(state));
            if (stateResult.IsFailure)
            {
                return stateResult.Error;
            }

            var cityResult = ValidateParameter(city, nameof(city));
            if (cityResult.IsFailure)
            {
                return cityResult.Error;
            }

            var streetResult = ValidateParameter(street, nameof(street));
            if (streetResult.IsFailure)
            {
                return streetResult.Error;
            }

            var buildingNumberResult = ValidateParameter(buildingNumber, nameof(buildingNumber));
            if (buildingNumberResult.IsFailure)
            {
                return buildingNumberResult.Error;
            }

            return new Address(countryResult.Value, stateResult.Value, cityResult.Value, streetResult.Value, buildingNumberResult.Value);
        }

        private static Result<string, AppError> ValidateParameter(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                return AppError.Validation($"address.{parameterName}", $"{parameterName} cannot be null or empty.");
            }

            var normalizedParameter = parameter.Trim();

            if (normalizedParameter.Length > MAX_LENGTH)
            {
                return AppError.Validation($"address.{parameterName}", $"{parameterName} exceeds the maximum length of {MAX_LENGTH} characters.");
            }

            return normalizedParameter;
        }
    }
}
