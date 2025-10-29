using Flow_Api.Data.Contexts;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Repositories.Interfaces;
using Flow_Api.Repositories.Interfaces.Master;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TenantEntity = Flow_Api.Models.Entities.Master.Tenant;


namespace Flow_Api.Repositories.Implementations.Master
{
    public class TenantRepository : BaseRepository<TenantEntity>, ITenantRepository
    {
        private new readonly MasterDbContext _context;

        public TenantRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TenantEntity>> GetPendingRegistrationsAsync()
        {
            return await _context.Tenants
                .Include(t => t.Owner)
                .Where(t => t.RegistrationStatus == RegistrationStatus.Pending)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TenantEntity?> GetByIdWithOwnerAsync(Guid id)
        {
            return await _context.Tenants
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> IsSchemaNameUniqueAsync(string schemaName)
        {
            return !await _context.Tenants.AnyAsync(t => t.SchemaName == schemaName);
        }
    }
}
