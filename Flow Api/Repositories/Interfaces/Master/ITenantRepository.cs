namespace Flow_Api.Repositories.Interfaces.Master
{
    using Flow_Api.Models.Entities.Master;
    using Flow_Api.Models.Entities.Enums;
    using TenantEntity = Flow_Api.Models.Entities.Master.Tenant;

    public interface ITenantRepository : IBaseRepository<TenantEntity>
    {
        Task<IEnumerable<TenantEntity>> GetPendingRegistrationsAsync();
        Task<TenantEntity?> GetByIdWithOwnerAsync(Guid id);
        Task<bool> IsSchemaNameUniqueAsync(string schemaName);
    }
}
