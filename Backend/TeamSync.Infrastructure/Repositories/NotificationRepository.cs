using MongoDB.Driver;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;

namespace TeamSync.Infrastructure.Repositories;

public class NotificationRepository
    : MongoRepository<Notification>, INotificationRepository
{
    public NotificationRepository(IMongoDatabase database)
        : base(database, "Notifications")
    {
    }

    public async Task<List<Notification>> GetByUserAsync(string userId)
    {
        return await Collection
            .Find(n => n.UserId == userId)
            .SortByDescending(n => n.CreatedAt)
            .Limit(50)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        var filter = Builders<Notification>.Filter.Eq(n => n.Id, notificationId);
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.UpdatedAt, DateTime.UtcNow);

        await Collection.UpdateOneAsync(filter, update);
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var filter = Builders<Notification>.Filter
            .Eq(n => n.UserId, userId);

        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.UpdatedAt, DateTime.UtcNow);

        await Collection.UpdateManyAsync(filter, update);
    }
}
