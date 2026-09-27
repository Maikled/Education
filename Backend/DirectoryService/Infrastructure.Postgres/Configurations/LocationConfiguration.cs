using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Configurations
{
    internal sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("locations");

            builder.HasKey(p => p.Id).HasName("pk_location");

            builder.Property(p => p.Name)
                .HasConversion(p => p.Value, value => Name.Create(value))
                .IsRequired()
                .HasMaxLength(Name.MAX_LENGTH)
                .HasColumnName("name");

            builder.OwnsOne(p => p.Address, address =>
            {
                address.ToJson();

                address.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(Address.MAX_LENGTH)
                    .HasJsonPropertyName("country");

                address.Property(a => a.State)
                    .IsRequired()
                    .HasMaxLength(Address.MAX_LENGTH)
                    .HasJsonPropertyName("state");

                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(Address.MAX_LENGTH)
                    .HasJsonPropertyName("city");

                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(Address.MAX_LENGTH)
                    .HasJsonPropertyName("street");

                address.Property(a => a.BuildingNumber)
                    .IsRequired()
                    .HasMaxLength(Address.MAX_LENGTH)
                    .HasJsonPropertyName("building_number");
            });

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false)
                .HasColumnName("updated_at");
        }
    }
}
