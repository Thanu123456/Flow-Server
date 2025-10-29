using Flow_Api.Models.Entities.Master;

namespace Flow_Api.Repositories.Interfaces.Master
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
    }
}
