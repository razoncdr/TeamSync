using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;

    public NotificationService(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<NotificationDto>> GetMyNotificationsAsync(string userId)
    {
        var notifications = await _repo.GetByUserAsync(userId);

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            Metadata = n.Metadata
        }).ToList();
    }

    public async Task MarkAsReadAsync(string notificationId, string userId)
    {
        await _repo.MarkAsReadAsync(notificationId);
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        await _repo.MarkAllAsReadAsync(userId);
    }

    public async Task CreateAsync(
        string userId,
        string title,
        string message,
        string type,
        Dictionary<string, string>? metadata = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Metadata = metadata
        };

        await _repo.AddAsync(notification);
    }
}
