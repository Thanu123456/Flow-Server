using Flow_Api.Models.Entities.Master;

namespace Flow_Api.Repositories.Interfaces.Master
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
