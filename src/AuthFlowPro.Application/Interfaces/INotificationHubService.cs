namespace AuthFlowPro.Application.Interfaces;

public interface INotificationHubService
{
    Task SendNotificationToUserAsync(Guid userId, object notification);
    Task SendNotificationToOrganizationAsync(Guid organizationId, object notification);
}