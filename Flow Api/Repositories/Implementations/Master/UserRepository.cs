using Flow_Api.Data.Contexts;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Repositories.Interfaces.Master;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Repositories.Implementations.Master
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private new readonly MasterDbContext _context;

        public UserRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
