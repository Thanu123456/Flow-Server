using Flow_Api.Models.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Flow_Api.Data.Seeds
{
    public class DefaultUsers
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<Program> logger)
        {
            // Create admin user if it doesn't exist
            var adminEmail = "admin@posapi.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    logger.LogInformation($"Admin user {adminEmail} created successfully");

                    // Assign admin role
                    if (await roleManager.RoleExistsAsync("Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation($"Admin role assigned to {adminEmail}");
                    }
                }
                else
                {
                    logger.LogError($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Create test user if it doesn't exist
            var testEmail = "user@posapi.com";
            var testUser = await userManager.FindByEmailAsync(testEmail);

            if (testUser == null)
            {
                testUser = new ApplicationUser
                {
                    UserName = testEmail,
                    Email = testEmail,
                    FirstName = "Test",
                    LastName = "User",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(testUser, "User@123456");
                if (result.Succeeded)
                {
                    logger.LogInformation($"Test user {testEmail} created successfully");

                    // Assign user role
                    if (await roleManager.RoleExistsAsync("User"))
                    {
                        await userManager.AddToRoleAsync(testUser, "User");
                        logger.LogInformation($"User role assigned to {testEmail}");
                    }
                }
                else
                {
                    logger.LogError($"Error creating test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
