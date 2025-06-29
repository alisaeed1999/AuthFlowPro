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

    public static async Task SeedPlansAsync(AppDbContext context)
    {
        if (!context.Plans.Any())
        {
            var plans = new List<Plan>
            {
                new Plan
                {
                    Id = "starter",
                    Name = "Starter",
                    Description = "Perfect for small teams getting started",
                    Price = 9.99m,
                    Currency = "USD",
                    Interval = PlanInterval.Monthly,
                    MaxUsers = 5,
                    MaxProjects = 10,
                    HasAdvancedFeatures = false,
                    IsActive = true
                },
                new Plan
                {
                    Id = "pro",
                    Name = "Professional",
                    Description = "For growing teams that need more features",
                    Price = 29.99m,
                    Currency = "USD",
                    Interval = PlanInterval.Monthly,
                    MaxUsers = 25,
                    MaxProjects = 100,
                    HasAdvancedFeatures = true,
                    IsActive = true
                },
                new Plan
                {
                    Id = "enterprise",
                    Name = "Enterprise",
                    Description = "For large organizations with advanced needs",
                    Price = 99.99m,
                    Currency = "USD",
                    Interval = PlanInterval.Monthly,
                    MaxUsers = -1, // Unlimited
                    MaxProjects = -1, // Unlimited
                    HasAdvancedFeatures = true,
                    IsActive = true
                }
            };

            context.Plans.AddRange(plans);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                Console.WriteLine("✅ Admin user created.");

                // Create roles if they don't exist and assign
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }

                await userManager.AddToRolesAsync(adminUser, roles);

                // Create a default organization for the admin
                var organization = new Organization
                {
                    Name = "Admin Organization",
                    Slug = "admin-org",
                    Description = "Default organization for admin user"
                };

                context.Organizations.Add(organization);

                var membership = new OrganizationMember
                {
                    OrganizationId = organization.Id,
                    UserId = adminUser.Id,
                    Role = OrganizationRole.Owner
                };

                context.OrganizationMembers.Add(membership);
                await context.SaveChangesAsync();

                Console.WriteLine("✅ Admin organization created.");
            }
            else
            {
                Console.WriteLine("❌ Failed to create admin user:");
                foreach (var error in result.Errors)
                    Console.WriteLine($" - {error.Description}");
                return;
            }
        }
        else
        {
            Console.WriteLine("ℹ️ Admin user already exists.");
        }
    }
}