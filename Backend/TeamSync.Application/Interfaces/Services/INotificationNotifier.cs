using TeamSync.Application.Events;

public interface INotificationNotifier
{
    Task TaskCreatedAsync(string userId, TaskCreatedEvent evt);
    //Task TaskUpdatedAsync(string userId, TaskUpdatedEvent evt);
    Task TaskAssignedAsync(string userId, TaskAssignedEvent evt);
    Task TaskUnassignedAsync(string userId, TaskUnassignedEvent evt);
}