using Flow_Api.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Repositories.Interfaces.Master;

namespace Flow_Api.Repositories.Implementations.Master
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        private new readonly MasterDbContext _context;

        public AuditLogRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Include(a => a.Tenant)
                .Where(a => a.TenantId == tenantId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Include(a => a.Tenant)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
