using Flow_Api.Data.Contexts;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Repositories.Interfaces.Master;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Repositories.Implementations.Master
{
    public class SystemSettingRepository : BaseRepository<SystemSetting>, ISystemSettingRepository
    {
        private new readonly MasterDbContext _context;

        public SystemSettingRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SystemSetting?> GetByKeyAsync(string key)
        {
            return await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
        }

        public async Task<IEnumerable<SystemSetting>> GetByCategoryAsync(string category)
        {
            return await _context.SystemSettings
                .Where(s => s.Category == category)
                .ToListAsync();
        }
    }
}
