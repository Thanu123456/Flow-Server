using Flow_Api.Models.Entities.Tenant.Management;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Data.Contexts
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

        // Tenant-specific DbSets will be added here
        // public DbSet<Product> Products { get; set; }
        // public DbSet<Sale> Sales { get; set; }
        // etc.

        //categories dbset
        public DbSet<Category> Categories => Set<Category>();
        
        //TODO: add subcategory Dbset

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Schema will be set dynamically via connection string or search_path

            modelBuilder.Entity<Category>()
                    .HasIndex(c => c.Code)
                    .IsUnique();

            modelBuilder.Entity<Category>()
                    .HasIndex(c => c.Name)
                    .IsUnique();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        }
    }
}
