using Microsoft.AspNetCore.SignalR;
using TeamSync.API.Hubs;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.API.Realtime
{
    public class SignalRNotifier :
        IChatNotifier,
        INotificationNotifier
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public SignalRNotifier(
            IHubContext<ChatHub> chatHub,
            IHubContext<NotificationHub> notificationHub)
        {
            _chatHub = chatHub;
            _notificationHub = notificationHub;
        }

        public async Task MessageCreatedAsync(
            string projectId,
            object messageDto)
        {
            await _chatHub.Clients
                .Group($"project:{projectId}")
                .SendAsync("ReceiveMessage", messageDto);
        }

        public async Task TaskCreatedAsync(string userId, TaskCreatedEvent evt)
        {
            await _notificationHub.Clients
                .Group($"user:{userId}")
                .SendAsync(
                    "ReceiveNotification",
                    "New task created",
                    $"Task '{evt.TaskId}' was added to the project",
                    new { evt.TaskId, evt.ProjectId }
                );
        }

        public Task TaskAssignedAsync(string userId, TaskAssignedEvent evt)
        {
            return _notificationHub.Clients.User(userId)
                .SendAsync("TaskAssigned", new
                {
                    evt.TaskId,
                    evt.ProjectId,
                    Message = "You were assigned to a task"
                });
        }

        public Task TaskUnassignedAsync(string userId, TaskUnassignedEvent evt)
        {
            return _notificationHub.Clients.User(userId)
                .SendAsync("TaskUnassigned", new
                {
                    evt.TaskId,
                    evt.ProjectId,
                    Message = "You were removed from a task"
                });
        }
    }
}
