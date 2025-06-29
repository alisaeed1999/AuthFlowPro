using AuthFlowPro.Application.DTOs.Notification;

namespace AuthFlowPro.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<(bool Success, string Message)> CreateNotificationAsync(CreateNotificationRequest request);
    Task<(bool Success, string Message)> MarkAsReadAsync(Guid userId, MarkNotificationReadRequest request);
    Task<(bool Success, string Message)> MarkAllAsReadAsync(Guid userId);
    Task<(bool Success, string Message)> DeleteNotificationAsync(Guid userId, Guid notificationId);
}