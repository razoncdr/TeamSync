using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetByUserAsync(string userId);
    Task MarkAsReadAsync(string notificationId);
    Task MarkAllAsReadAsync(string userId);
}
