using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using AuthFlowPro.Infrastructure.Data;
using AuthFlowPro.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthFlowPro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INotificationHubService, NotificationHubService>();
        
        // Use enhanced notification service with SignalR support
        services.AddScoped<INotificationService, EnhancedNotificationService>();

        return services;
    }
}