using AuthFlowPro.Domain.Entities;

namespace AuthFlowPro.Application.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActionUrl { get; set; }
}

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public string? ActionUrl { get; set; }
}

public class MarkNotificationReadRequest
{
    public List<Guid> NotificationIds { get; set; } = new();
}