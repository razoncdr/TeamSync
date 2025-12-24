using TeamSync.Application.DTOs;

namespace TeamSync.Application.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationDto>> GetMyNotificationsAsync(string userId);
    Task MarkAsReadAsync(string notificationId, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task CreateAsync(
        string userId,
        string title,
        string message,
        string type,
        Dictionary<string, string>? metadata = null
    );
}
