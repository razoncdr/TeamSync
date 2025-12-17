using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs.Project;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;
using TeamSync.Domain.Enums;

namespace TeamSync.Application.Services
{
	public class ChatService : IChatService
    {
		private readonly IChatRepository _chatRepository;
		private readonly IUserRepository _userRepository;

        public ChatService(
			IChatRepository chatRepository,
			IUserRepository userRepository
			)
		{
            _chatRepository = chatRepository;
			_userRepository = userRepository;
        }

        public async Task<List<ChatMessage>> GetProjectChatsAsync(string projectId)
        {
			var chats = await _chatRepository.GetByProjectIdAsync(projectId);
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
			return chatMessage;
        }
    }
}
