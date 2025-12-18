using Microsoft.AspNetCore.SignalR;
using TeamSync.API.Hubs;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.API.Realtime
{
    public class SignalRChatNotifier : IChatNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRChatNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyMessageCreatedAsync(
            string projectId,
            object messageDto)
        {
            await _hubContext.Clients
                .Group($"project:{projectId}")
                .SendAsync("ReceiveMessage", messageDto);
        }
    }
}
