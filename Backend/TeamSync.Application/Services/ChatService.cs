using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services
{
	public class ChatService : IChatService
    {
		private readonly IChatRepository _chatRepository;
		private readonly IUserRepository _userRepository;
        private readonly IChatNotifier _chatNotifier;

        public ChatService(
			IChatRepository chatRepository,
			IUserRepository userRepository,
            IChatNotifier chatNotifier
            )
		{
            _chatRepository = chatRepository;
			_userRepository = userRepository;
			_chatNotifier = chatNotifier;
        }

        public async Task<List<ChatMessage>> GetProjectChatsAsync(string projectId, int skip, int limit)
        {
			var chats = await _chatRepository.GetPaginatedByProjectIdAsync(projectId, skip, limit);
            chats.Reverse();
			return chats;
        }

		public async Task<ChatMessage> CreateMessageAsync(string projectId, string senderId, string message)
		{
			var chatMessage = new ChatMessage
			{
				ProjectId = projectId,
				SenderId = senderId,
				SenderName = _userRepository.GetByIdAsync(senderId).Result.Name,
                Message = message,
				CreatedAt = DateTime.UtcNow
			};
			await _chatRepository.AddAsync(chatMessage);
            await _chatNotifier.NotifyMessageCreatedAsync(
                projectId,
                new
                {
                    id = chatMessage.Id,
                    senderId = chatMessage.SenderId,
                    senderName = chatMessage.SenderName,
                    message = chatMessage.Message,
                    createdAt = chatMessage.CreatedAt
                }
            );
            return chatMessage;
        }
    }
}
