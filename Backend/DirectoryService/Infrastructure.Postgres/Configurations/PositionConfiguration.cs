using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Configurations
{
    internal sealed class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("positions");

            builder.HasKey(p => p.Id).HasName("pk_position");

            builder.Property(p => p.Name)
                .HasConversion(p => p.Value, value => Name.Create(value))
                .IsRequired()
                .HasMaxLength(Name.MAX_LENGTH)
                .HasColumnName("name");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false)
                .HasColumnName("updated_at");
        }
    }
}
