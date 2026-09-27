using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Configurations
{
    internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("departments");

            builder.HasKey(p => p.Id)
                .HasName("pk_department");

            builder.Property(p => p.ParentId)
                .IsRequired(false)
                .HasColumnName("parent_id");

            builder.HasOne<Department>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_department_parent");

            builder.Property(p => p.Name)
                .HasConversion(p => p.Value, value => Name.Create(value))
                .IsRequired()
                .HasMaxLength(Name.MAX_LENGTH)
                .HasColumnName("name");

            builder.Property(p => p.Slug)
                .HasConversion(p => p.Value, value => Slug.Create(value))
                .IsRequired()
                .HasMaxLength(Slug.MAX_LENGTH)
                .HasColumnName("slug");

            builder.Property(p => p.Path)
                .HasConversion(p => p.Value, value => DepartmentPath.Create(Slug.Create(value)))
                .IsRequired()
                .HasColumnName("path");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false)
                .HasColumnName("updated_at");
        }
    }
}
