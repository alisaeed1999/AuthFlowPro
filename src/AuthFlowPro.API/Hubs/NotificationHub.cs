using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AuthFlowPro.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinUserGroup()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
    }

    public async Task JoinOrganizationGroup(string organizationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"org_{organizationId}");
    }

    public async Task LeaveOrganizationGroup(string organizationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"org_{organizationId}");
    }

    public override async Task OnConnectedAsync()
    {
        await JoinUserGroup();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}