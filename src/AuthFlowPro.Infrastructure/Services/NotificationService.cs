using AuthFlowPro.Application.DTOs.Notification;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using AuthFlowPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using AuthFlowPro.API.Hubs;

namespace AuthFlowPro.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(AppDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50) // Limit to recent notifications
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ActionUrl = n.ActionUrl
            })
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<(bool Success, string Message)> CreateNotificationAsync(CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            UserId = request.UserId,
            OrganizationId = request.OrganizationId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            ActionUrl = request.ActionUrl
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Send real-time notification via SignalR
        await _hubContext.Clients.Group($"user_{request.UserId}")
            .SendAsync("ReceiveNotification", new
            {
                id = notification.Id.ToString(),
                title = notification.Title,
                message = notification.Message,
                type = notification.Type.ToString().ToLower(),
                timestamp = notification.CreatedAt,
                actionUrl = notification.ActionUrl
            });

        return (true, "Notification created successfully");
    }

    public async Task<(bool Success, string Message)> MarkAsReadAsync(Guid userId, MarkNotificationReadRequest request)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && request.NotificationIds.Contains(n.Id) && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return (true, $"Marked {notifications.Count} notifications as read");
    }

    public async Task<(bool Success, string Message)> MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return (true, $"Marked {notifications.Count} notifications as read");
    }

    public async Task<(bool Success, string Message)> DeleteNotificationAsync(Guid userId, Guid notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null)
        {
            return (false, "Notification not found");
        }

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        return (true, "Notification deleted successfully");
    }
}