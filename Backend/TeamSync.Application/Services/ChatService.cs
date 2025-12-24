using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IChatNotifier _chatNotifier;
        private readonly IRedisCacheService _redis;

        public ChatService(
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IProjectMemberRepository projectMemberRepository,
            IChatNotifier chatNotifier,
            IRedisCacheService redis
        )
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _projectMemberRepository = projectMemberRepository;
            _chatNotifier = chatNotifier;
            _redis = redis;
        }

        public async Task<List<ChatMessage>> GetProjectChatsAsync(string userId, string projectId, int skip, int limit)
        {
            // Check if sender is a project member
            var isMember = await _projectMemberRepository.ExistsByUserIdAsync(projectId, userId);
            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this project.");

            // Read from Redis only for the first page
            if (skip == 0)
            {
                var chatsCacheKey = $"chat:project:{projectId}";
                var chats = await _redis.ListRangeAsync<ChatMessage>(chatsCacheKey);

                // Cache miss
                if (chats == null || chats.Count == 0)
                {
                    chats = await _chatRepository.GetPaginatedByProjectIdAsync(projectId, 0, 20);
                    chats.Reverse(); // latest at the end

                    // Store in Redis list
                    foreach (var msg in chats)
                    {
                        await _redis.ListRightPushAsync(chatsCacheKey, msg);
                    }

                    // Keep only latest 20
                    await _redis.ListTrimAsync(chatsCacheKey, -20, -1);
                }

                return chats;
            }

            // For older pages, read directly from MongoDB
            var olderChats = await _chatRepository.GetPaginatedByProjectIdAsync(projectId, skip, limit);
            olderChats.Reverse();
            return olderChats;
        }

        public async Task<ChatMessage> CreateMessageAsync(string projectId, string senderId, string message)
        {
            // Check if sender is a project member
            var isMember = await _projectMemberRepository.ExistsByUserIdAsync(projectId, senderId);
            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this project.");

            var senderName = (await _userRepository.GetByIdAsync(senderId)).Name;
            var chatMessage = new ChatMessage
            {
                ProjectId = projectId,
                SenderId = senderId,
                SenderName = senderName,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            // Save to MongoDB
            await _chatRepository.AddAsync(chatMessage);

            // Redis operations: add new message & keep only latest 20
            var chatsCacheKey = $"chat:project:{projectId}";
            await _redis.ListRightPushAsync(chatsCacheKey, chatMessage);
            await _redis.ListTrimAsync(chatsCacheKey, -20, -1);

            // Notify SignalR group
            await _chatNotifier.MessageCreatedAsync(
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
