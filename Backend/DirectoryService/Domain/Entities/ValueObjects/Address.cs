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

        public static Address Create(string country, string state, string city, string street, string buildingNumber)
        {
            return new Address(ValidateParameter(country, nameof(country)), ValidateParameter(state, nameof(state)), ValidateParameter(city, nameof(city)), ValidateParameter(street, nameof(street)), ValidateParameter(buildingNumber, nameof(buildingNumber)));
        }

        private static string ValidateParameter(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(parameterName, $"{parameterName} cannot be null or empty.");
            }

            var normalizedParameter = parameter.Trim();

            if (normalizedParameter.Length > MAX_LENGTH)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} exceeds the maximum length of {MAX_LENGTH} characters.");
            }

            return normalizedParameter;
        }
    }
}
