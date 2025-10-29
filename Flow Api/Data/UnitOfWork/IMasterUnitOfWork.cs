using Flow_Api.Repositories.Interfaces.Master;

namespace Flow_Api.Data.UnitOfWork
{
    public interface IMasterUnitOfWork : IDisposable
    {
        ITenantRepository Tenants { get; }
        IUserRepository Users { get; }
        IAuditLogRepository AuditLogs { get; }
        ISystemSettingRepository SystemSettings { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
