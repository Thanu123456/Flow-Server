using Flow_Api.Models.Entities.Tenant.Management;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Data.Contexts
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) 
            : base(options) 
        { }

        // DbSets
        public DbSet<Brand> Brands => Set<Brand>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Fallback – usually configured in Program.cs
            }
        }
    }
}