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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Schema will be set dynamically via connection string or search_path
        }
    }
}
