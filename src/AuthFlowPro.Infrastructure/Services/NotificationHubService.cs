using AuthFlowPro.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AuthFlowPro.Infrastructure.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<Hub> _hubContext;

    public NotificationHubService(IHubContext<Hub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(Guid userId, object notification)
    {
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("ReceiveNotification", notification);
    }

    public async Task SendNotificationToOrganizationAsync(Guid organizationId, object notification)
    {
        await _hubContext.Clients.Group($"org_{organizationId}")
            .SendAsync("ReceiveNotification", notification);
    }
}