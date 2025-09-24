using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Flow_Api.Models.Identity;
using System.Threading.Tasks;

namespace Flow_Api.Data.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager, ILogger<Program> logger)
        {
            // Create roles if they don't exist
            string[] roleNames = { "Admin", "Manager", "Cashier", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        Description = $"Default {roleName} role"
                    };

                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role {roleName} created successfully");
                    }
                    else
                    {
                        logger.LogError($"Error creating role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}
