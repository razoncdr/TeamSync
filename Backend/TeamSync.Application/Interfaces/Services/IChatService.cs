using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IChatService
    {
		Task<List<ChatMessage>> GetProjectChatsAsync(string projectId, int skip, int limit);
		Task<ChatMessage> CreateMessageAsync(string projectId, string senderId, string message);
    }
}
