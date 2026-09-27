using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Infrastructure.Postgres.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres
{
    public class AppDbContext : DbContext
    {
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<DepartmentLocation> DepartmentLocations { get; set; } = null!;
        public DbSet<DepartmentPosition> DepartmentPositions { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Department>(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration<Location>(new LocationConfiguration());
            modelBuilder.ApplyConfiguration<Position>(new PositionConfiguration());
            modelBuilder.ApplyConfiguration<DepartmentLocation>(new DepartmentLocationsConfiguration());
            modelBuilder.ApplyConfiguration<DepartmentPosition>(new DepartmentPositionsConfiguration());
        }
    }
}
