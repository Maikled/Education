using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Configurations
{
    internal sealed class DepartmentPositionsConfiguration : IEntityTypeConfiguration<DepartmentPosition>
    {
        public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
        {
            builder.ToTable("department_positions");

            builder.HasKey(p => p.Id).HasName("pk_department_position");

            builder.Property(p => p.DepartmentId)
                .IsRequired()
                .HasColumnName("department_id");

            builder.Property(p => p.PositionId)
                .IsRequired()
                .HasColumnName("position_id");

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_position_department");

            builder.HasOne<Position>()
                .WithMany()
                .HasForeignKey(p => p.PositionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_position_position");

            builder.HasIndex(p => new { p.DepartmentId, p.PositionId })
                .IsUnique()
                .HasDatabaseName("ux_department_position_department_position");
        }
    }
}
