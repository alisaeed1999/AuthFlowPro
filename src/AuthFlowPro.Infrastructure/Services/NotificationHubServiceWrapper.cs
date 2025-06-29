using AuthFlowPro.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AuthFlowPro.Infrastructure.Services;

public class NotificationHubServiceWrapper : INotificationHubService
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationHubServiceWrapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendNotificationToUserAsync(Guid userId, object notification)
    {
        // Get the hub context dynamically to avoid circular dependency
        var hubContext = _serviceProvider.GetService<IHubContext<Hub>>();
        if (hubContext != null)
        {
            await hubContext.Clients.Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notification);
        }
    }

    public async Task SendNotificationToOrganizationAsync(Guid organizationId, object notification)
    {
        // Get the hub context dynamically to avoid circular dependency
        var hubContext = _serviceProvider.GetService<IHubContext<Hub>>();
        if (hubContext != null)
        {
            await hubContext.Clients.Group($"org_{organizationId}")
                .SendAsync("ReceiveNotification", notification);
        }
    }
}