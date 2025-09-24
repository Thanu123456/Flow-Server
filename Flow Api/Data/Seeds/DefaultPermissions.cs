using Flow_Api.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Flow_Api.Data.Seeds
{
    public class DefaultPermissions
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager, ILogger<Program> logger)
        {
            // Define permissions
            var permissions = new[]
            {
                new { Name = "users.view", Description = "View users", Category = "Users" },
                new { Name = "users.create", Description = "Create users", Category = "Users" },
                new { Name = "users.edit", Description = "Edit users", Category = "Users" },
                new { Name = "users.delete", Description = "Delete users", Category = "Users" },
                new { Name = "roles.view", Description = "View roles", Category = "Roles" },
                new { Name = "roles.create", Description = "Create roles", Category = "Roles" },
                new { Name = "roles.edit", Description = "Edit roles", Category = "Roles" },
                new { Name = "roles.delete", Description = "Delete roles", Category = "Roles" },
                new { Name = "customers.view", Description = "View customers", Category = "Customers" },
                new { Name = "customers.create", Description = "Create customers", Category = "Customers" },
                new { Name = "customers.edit", Description = "Edit customers", Category = "Customers" },
                new { Name = "customers.delete", Description = "Delete customers", Category = "Customers" },
                new { Name = "suppliers.view", Description = "View suppliers", Category = "Suppliers" },
                new { Name = "suppliers.create", Description = "Create suppliers", Category = "Suppliers" },
                new { Name = "suppliers.edit", Description = "Edit suppliers", Category = "Suppliers" },
                new { Name = "suppliers.delete", Description = "Delete suppliers", Category = "Suppliers" },
                new { Name = "products.view", Description = "View products", Category = "Products" },
                new { Name = "products.create", Description = "Create products", Category = "Products" },
                new { Name = "products.edit", Description = "Edit products", Category = "Products" },
                new { Name = "products.delete", Description = "Delete products", Category = "Products" },
                new { Name = "purchases.view", Description = "View purchases", Category = "Purchases" },
                new { Name = "purchases.create", Description = "Create purchases", Category = "Purchases" },
                new { Name = "purchases.edit", Description = "Edit purchases", Category = "Purchases" },
                new { Name = "purchases.delete", Description = "Delete purchases", Category = "Purchases" },
                new { Name = "sales.view", Description = "View sales", Category = "Sales" },
                new { Name = "sales.create", Description = "Create sales", Category = "Sales" },
                new { Name = "sales.edit", Description = "Edit sales", Category = "Sales" },
                new { Name = "sales.delete", Description = "Delete sales", Category = "Sales" },
                new { Name = "reports.view", Description = "View reports", Category = "Reports" },
                new { Name = "dashboard.view", Description = "View dashboard", Category = "Dashboard" },
                new { Name = "barcode.generate", Description = "Generate barcodes", Category = "Barcode" },
                new { Name = "settings.view", Description = "View settings", Category = "Settings" },
                new { Name = "settings.edit", Description = "Edit settings", Category = "Settings" }
            };

            // Assign permissions to roles
            var rolePermissions = new[]
            {
                new { RoleName = "Admin", PermissionNames = permissions.Select(p => p.Name).ToArray() },
                new { RoleName = "Manager", PermissionNames = new[] {
                    "customers.view", "customers.create", "customers.edit", "customers.delete",
                    "suppliers.view", "suppliers.create", "suppliers.edit", "suppliers.delete",
                    "products.view", "products.create", "products.edit", "products.delete",
                    "purchases.view", "purchases.create", "purchases.edit", "purchases.delete",
                    "sales.view", "sales.create", "sales.edit", "sales.delete",
                    "reports.view", "dashboard.view", "barcode.generate"
                }},
                new { RoleName = "Cashier", PermissionNames = new[] {
                    "customers.view", "customers.create", "customers.edit",
                    "products.view",
                    "purchases.view", "purchases.create",
                    "sales.view", "sales.create", "sales.edit",
                    "dashboard.view", "barcode.generate"
                }},
                new { RoleName = "User", PermissionNames = new[] {
                    "customers.view",
                    "products.view",
                    "dashboard.view"
                }}
            };

            // Add permissions to roles
            foreach (var rolePermission in rolePermissions)
            {
                var role = await roleManager.FindByNameAsync(rolePermission.RoleName);
                if (role != null)
                {
                    foreach (var permissionName in rolePermission.PermissionNames)
                    {
                        // Check if the permission already exists
                        var existingClaims = await roleManager.GetClaimsAsync(role);
                        var claimExists = existingClaims.Any(c => c.Type == "Permission" && c.Value == permissionName);

                        if (!claimExists)
                        {
                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permissionName));
                            logger.LogInformation($"Permission {permissionName} added to role {rolePermission.RoleName}");
                        }
                    }
                }
            }
        }
    }
}
