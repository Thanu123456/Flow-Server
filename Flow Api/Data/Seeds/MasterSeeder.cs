using Flow_Api.Data.Contexts;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Services.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Data.Seeds
{
    public class MasterSeeder
    {
        private readonly MasterDbContext _context;
        private readonly IPasswordService _passwordService;

        public MasterSeeder(MasterDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            // Check if super admin already exists
            var superAdminExists = await _context.Users.AnyAsync(u => u.IsSuperAdmin);

            if (!superAdminExists)
            {
                var superAdmin = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "System Administrator",
                    Email = "superadmin@flowpos.com",
                    PhoneNumber = "+94771234567",
                    PasswordHash = _passwordService.HashPassword("SuperAdmin@123"),
                    IsSuperAdmin = true,
                    Status = UserStatus.Active,
                    MustChangePassword = true,
                    IsMfaEnabled = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(superAdmin);
            }

            // Seed default system settings
            var settingsExist = await _context.SystemSettings.AnyAsync();

            if (!settingsExist)
            {
                var defaultSettings = new List<SystemSetting>
                {
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SystemName",
                        Value = "Flow POS System",
                        Description = "Name of the POS system",
                        Category = "General",
                        CreatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SupportEmail",
                        Value = "support@flowpos.com",
                        Description = "Support contact email",
                        Category = "General",
                        CreatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "DefaultCurrency",
                        Value = "LKR",
                        Description = "Default currency for new tenants",
                        Category = "Tenant",
                        CreatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "DefaultTimezone",
                        Value = "Asia/Colombo",
                        Description = "Default timezone for new tenants",
                        Category = "Tenant",
                        CreatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "MaxFailedLoginAttempts",
                        Value = "5",
                        Description = "Maximum failed login attempts before lockout",
                        Category = "Security",
                        CreatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "AccountLockoutDuration",
                        Value = "15",
                        Description = "Account lockout duration in minutes",
                        Category = "Security",
                        CreatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SessionTimeout",
                        Value = "30",
                        Description = "Session timeout in minutes",
                        Category = "Security",
                        CreatedAt = DateTime.UtcNow
                    },
                    new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "PasswordExpiryDays",
                        Value = "90",
                        Description = "Password expiry period in days",
                        Category = "Security",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await _context.SystemSettings.AddRangeAsync(defaultSettings);
            }

            await _context.SaveChangesAsync();
        }
    }
}
