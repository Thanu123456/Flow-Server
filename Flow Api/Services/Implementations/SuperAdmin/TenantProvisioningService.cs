using Flow_Api.Data.Contexts;
using Flow_Api.Helpers;
using Flow_Api.Services.Interfaces.SuperAdmin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Flow_Api.Services.Implementations.SuperAdmin
{
    public class TenantProvisioningService : ITenantProvisioningService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuditLogService _auditLogService;

        public TenantProvisioningService(
            IConfiguration configuration,
            IAuditLogService auditLogService)
        {
            _configuration = configuration;
            _auditLogService = auditLogService;
        }

        public async Task<string> CreateTenantSchemaAsync(Guid tenantId, string shopName)
        {
            var schemaName = SchemaHelper.GenerateSchemaName(shopName, tenantId);
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Create schema
            await using var command = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS {schemaName}", connection);
            await command.ExecuteNonQueryAsync();

            // Log schema creation
            await _auditLogService.LogActionAsync(
                Models.Entities.Enums.AuditActionType.SchemaCreated,
                $"Schema {schemaName} created for tenant {tenantId}",
                null,
                tenantId
            );

            return schemaName;
        }

        public async Task RunMigrationsAsync(string schemaName)
        {
            // Create tenant-specific DbContext with schema
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsHistoryTable("__EFMigrationsHistory", schemaName));

            await using var context = new TenantDbContext(optionsBuilder.Options);

            // Set search path to tenant schema
            await context.Database.ExecuteSqlAsync($"SET search_path TO {schemaName}");

            // Run migrations
            await context.Database.MigrateAsync();

            await _auditLogService.LogActionAsync(
                Models.Entities.Enums.AuditActionType.MigrationExecuted,
                $"Migrations executed for schema {schemaName}",
                null,
                null
            );
        }

        public async Task SeedDefaultDataAsync(string schemaName, Guid tenantId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Set search path
            await using var setPathCommand = new NpgsqlCommand($"SET search_path TO {schemaName}", connection);
            await setPathCommand.ExecuteNonQueryAsync();

            // Seed default roles
            var seedRolesSql = @"
                INSERT INTO roles (id, name, is_system_role, status, created_at) VALUES
                (gen_random_uuid(), 'Owner', true, 1, NOW()),
                (gen_random_uuid(), 'Manager', true, 1, NOW()),
                (gen_random_uuid(), 'Cashier', true, 1, NOW()),
                (gen_random_uuid(), 'Stock Keeper', true, 1, NOW())
                ON CONFLICT DO NOTHING;
            ";

            await using var seedCommand = new NpgsqlCommand(seedRolesSql, connection);
            await seedCommand.ExecuteNonQueryAsync();

            // Add more default data as needed (payment methods, etc.)
        }
    }
}
