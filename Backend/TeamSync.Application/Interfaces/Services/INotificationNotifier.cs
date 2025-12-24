using TeamSync.Application.Events;

public interface INotificationNotifier
{
    Task TaskCreatedAsync(string userId, TaskCreatedEvent evt);
}