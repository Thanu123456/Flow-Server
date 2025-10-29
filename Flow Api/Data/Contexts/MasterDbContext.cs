using Flow_Api.Models.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Data.Contexts
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<SystemSetting> SystemSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("public");

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDbContext).Assembly);

            // Global query filters for soft delete
            modelBuilder.Entity<Tenant>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
