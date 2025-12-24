using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TeamSync.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task JoinUser()
        {
            // Use authenticated userId
            var userId = Context.UserIdentifier!;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        }

        public async Task JoinProject(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"project:{projectId}");
        }
    }
}
