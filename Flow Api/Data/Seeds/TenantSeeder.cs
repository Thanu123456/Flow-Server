using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Flow_Api.Data.Seeds
{
    public class TenantSeeder
    {
        private readonly IConfiguration _configuration;

        public TenantSeeder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SeedTenantSchemaAsync(string schemaName, Guid tenantId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Set search path to tenant schema
            await using var setPathCommand = new NpgsqlCommand($"SET search_path TO {schemaName}", connection);
            await setPathCommand.ExecuteNonQueryAsync();

            // Seed default roles
            var seedRolesSql = $@"
                INSERT INTO roles (id, name, is_system_role, status, created_at, is_deleted) VALUES
                (gen_random_uuid(), 'Owner', true, 1, NOW(), false),
                (gen_random_uuid(), 'Manager', true, 1, NOW(), false),
                (gen_random_uuid(), 'Cashier', true, 1, NOW(), false),
                (gen_random_uuid(), 'Stock Keeper', true, 1, NOW(), false)
                ON CONFLICT DO NOTHING;
            ";

            await using var seedRolesCommand = new NpgsqlCommand(seedRolesSql, connection);
            await seedRolesCommand.ExecuteNonQueryAsync();

            // Seed default payment methods
            var seedPaymentMethodsSql = $@"
                INSERT INTO payment_methods (id, name, is_active, created_at, is_deleted) VALUES
                (gen_random_uuid(), 'Cash', true, NOW(), false),
                (gen_random_uuid(), 'Card', true, NOW(), false),
                (gen_random_uuid(), 'Cheque', true, NOW(), false),
                (gen_random_uuid(), 'Bank Transfer', true, NOW(), false),
                (gen_random_uuid(), 'Credit', true, NOW(), false)
                ON CONFLICT DO NOTHING;
            ";

            await using var seedPaymentCommand = new NpgsqlCommand(seedPaymentMethodsSql, connection);
            await seedPaymentCommand.ExecuteNonQueryAsync();

            // Seed default units
            var seedUnitsSql = $@"
                INSERT INTO units (id, name, short_name, status, created_at, is_deleted) VALUES
                (gen_random_uuid(), 'Piece', 'Pcs', 1, NOW(), false),
                (gen_random_uuid(), 'Kilogram', 'Kg', 1, NOW(), false),
                (gen_random_uuid(), 'Gram', 'g', 1, NOW(), false),
                (gen_random_uuid(), 'Liter', 'L', 1, NOW(), false),
                (gen_random_uuid(), 'Milliliter', 'ml', 1, NOW(), false),
                (gen_random_uuid(), 'Box', 'Box', 1, NOW(), false),
                (gen_random_uuid(), 'Pack', 'Pack', 1, NOW(), false)
                ON CONFLICT DO NOTHING;
            ";

            await using var seedUnitsCommand = new NpgsqlCommand(seedUnitsSql, connection);
            await seedUnitsCommand.ExecuteNonQueryAsync();
        }
    }
}
