using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Configurations
{
    internal sealed class DepartmentLocationsConfiguration : IEntityTypeConfiguration<DepartmentLocation>
    {
        public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
        {
            builder.ToTable("department_locations");

            builder.HasKey(p => p.Id).HasName("pk_department_location");

            builder.Property(p => p.DepartmentId)
                .IsRequired()
                .HasColumnName("department_id");

            builder.Property(p => p.LocationId)
                .IsRequired()
                .HasColumnName("location_id");

            builder.Property(p => p.IsPrimary)
                .IsRequired()
                .HasColumnName("is_primary");

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_location_department");
                
            builder.HasOne<Location>()
                .WithMany()
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_location_location");

            builder.HasIndex(p => new { p.DepartmentId, p.LocationId })
                .IsUnique()
                .HasDatabaseName("ux_department_location_department_location");

            builder.HasIndex(p => p.DepartmentId)
                .IsUnique()
                .HasFilter("is_primary = true")
                .HasDatabaseName("ux_department_location_primary");
        }
    }
}
