using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IChatService
    {
		Task<List<ChatMessage>> GetProjectChatsAsync(string projectId);
		Task<ChatMessage> CreateMessageAsync(string projectId, string senderId, string message);
    }
}
