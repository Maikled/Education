namespace Domain.Entities.ValueObjects
{
    public record Address
    {
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
            if (string.IsNullOrEmpty(country))
            {
                throw new ArgumentNullException(nameof(country), "Country cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state), "State cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(city))
            {
                throw new ArgumentNullException(nameof(city), "City cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(street))
            {
                throw new ArgumentNullException(nameof(street), "Street cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(buildingNumber))
            {
                throw new ArgumentNullException(nameof(buildingNumber), "Building number cannot be null or empty.");
            }

            return new Address(country, state, city, street, buildingNumber);
        }
    }
}
