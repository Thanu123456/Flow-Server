using Flow_Api.Data.Contexts;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Services.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;
using TenantEntity = Flow_Api.Models.Entities.Master.Tenant;

namespace Flow_Api.Data.Seeds
{
    public class DevelopmentSeeder
    {
        private readonly MasterDbContext _context;
        private readonly IPasswordService _passwordService;

        public DevelopmentSeeder(MasterDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            // Only seed in development environment

            // Check if test data already exists
            var testOwnerExists = await _context.Users.AnyAsync(u =>
                u.Email == "testowner@example.com");

            if (testOwnerExists)
                return;

            // Create test owner
            var testOwner = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Test Owner",
                Email = "testowner@example.com",
                PhoneNumber = "+94771234567",
                PasswordHash = _passwordService.HashPassword("Test@123"),
                IsSuperAdmin = false,
                Status = UserStatus.Active,
                MustChangePassword = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(testOwner);
            await _context.SaveChangesAsync();

            // Create test tenant
            var testTenant = new TenantEntity
            {
                Id = Guid.NewGuid(),
                ShopName = "Test Store",
                BusinessType = "Retail",
                AddressLine1 = "123 Test Street",
                City = "Colombo",
                Country = "Sri Lanka",
                OwnerId = testOwner.Id,
                RegistrationStatus = RegistrationStatus.Active,
                SchemaName = "tenant_teststore_12345678",
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Tenants.AddAsync(testTenant);

            // Link tenant to owner
            testOwner.TenantId = testTenant.Id;

            await _context.SaveChangesAsync();
        }
    }
}
