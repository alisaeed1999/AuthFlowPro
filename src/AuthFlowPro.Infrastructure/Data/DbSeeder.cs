using Microsoft.AspNetCore.Identity;
using AuthFlowPro.Application.Permission;
using AuthFlowPro.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace AuthFlowPro.Infrastructure.Data;

public class DbSeeder
{
    public static async Task SeedDefaultRolesAndPermissionsAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in RolePermissions.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var identityRole = new IdentityRole<Guid>(role);
                await roleManager.CreateAsync(identityRole);

                // Add role claims (permissions)
                foreach (var permission in RolePermissions.PermissionsByRole[role])
                {
                    await roleManager.AddClaimAsync(identityRole, new System.Security.Claims.Claim("permission", permission));
                }
            }
        }
    }


    public static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var adminConfig = configuration.GetSection("AdminUser");
        var email = adminConfig["Email"];
        var password = adminConfig["Password"];
        var roles = adminConfig.GetSection("Roles").Get<List<string>>() ?? new List<string>();

        if (email == null || password == null || roles.Count == 0)
        {
            Console.WriteLine("❌ Admin user config is invalid.");
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                Console.WriteLine("✅ Admin user created.");
            }
            else
            {
                Console.WriteLine("❌ Failed to create admin user:");
                foreach (var error in result.Errors)
                    Console.WriteLine($" - {error.Description}");
                return;
            }

            // Create roles if they don't exist and assign
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            await userManager.AddToRolesAsync(adminUser, roles);
        }
        else
        {
            Console.WriteLine("ℹ️ Admin user already exists.");
        }
    }
}
