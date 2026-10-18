using Contracts.Errors;
using CSharpFunctionalExtensions;
using Domain.Entities.ValueObjects;

namespace Domain.Entities
{
    public class Location
    {
        public Guid Id { get; private set; }
        public Name Name { get; private set; } = null!;
        public Address Address { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Location() { } // Required by EF Core

        private Location(Name name, Address address) : this()
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Address = address;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Location, AppError> Create(Name name, Address address)
        {
            var validationResult = Validate(name, address);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Location>();

            return new Location(name, address);
        }

        public UnitResult<AppError> Update(Name name, Address address)
        {
            var validationResult = Validate(name, address);
            if (validationResult.IsFailure)
                return validationResult;

            Name = name;
            Address = address;
            UpdatedAt = DateTime.UtcNow;

            return UnitResult.Success<AppError>();
        }

        public static UnitResult<AppError> Validate(Name name, Address address)
        {
            return Result.Combine(errors => AppError.FromErrors(errors), ValidateName(name), ValidateAddress(address));
        }

        private static UnitResult<AppError> ValidateName(Name name)
        {
            return UnitResult.FailureIf(name == null, AppError.Validation("location.name", "Location name cannot be null."));
        }

        private static UnitResult<AppError> ValidateAddress(Address address)
        {
            return UnitResult.FailureIf(address == null, AppError.Validation("location.address", "Location address cannot be null."));
        }
    }
}
