using Flow_Api.Data.Contexts;
using Flow_Api.Repositories.Interfaces.Master;
using Flow_Api.Repositories.Implementations.Master;
using Microsoft.EntityFrameworkCore.Storage;

namespace Flow_Api.Data.UnitOfWork
{
    public class MasterUnitOfWork : IMasterUnitOfWork
    {
        private readonly MasterDbContext _context;
        private IDbContextTransaction? _transaction;

        public ITenantRepository Tenants { get; }
        public IUserRepository Users { get; }
        public IAuditLogRepository AuditLogs { get; }
        public ISystemSettingRepository SystemSettings { get; }

        public MasterUnitOfWork(
            MasterDbContext context,
            ITenantRepository tenants,
            IUserRepository users,
            IAuditLogRepository auditLogs,
            ISystemSettingRepository systemSettings)
        {
            _context = context;
            Tenants = tenants;
            Users = users;
            AuditLogs = auditLogs;
            SystemSettings = systemSettings;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
