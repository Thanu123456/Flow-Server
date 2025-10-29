namespace Flow_Api.Services.Interfaces.SuperAdmin
{
    public interface ITenantProvisioningService
    {
        Task<string> CreateTenantSchemaAsync(Guid tenantId, string shopName);
        Task RunMigrationsAsync(string schemaName);
        Task SeedDefaultDataAsync(string schemaName, Guid tenantId);
    }
}
