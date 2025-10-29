using Flow_Api.Models.Entities.Master;

namespace Flow_Api.Repositories.Interfaces.Master
{
    public interface ISystemSettingRepository : IBaseRepository<SystemSetting>
    {
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task<IEnumerable<SystemSetting>> GetByCategoryAsync(string category);
    }
}
