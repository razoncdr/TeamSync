using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;
using MongoDB.Driver;

namespace TeamSync.Infrastructure.Repositories
{
	public class ChatRepository : MongoRepository<ChatMessage>, IChatRepository
    {
		public ChatRepository(IMongoDatabase database)
			: base(database, "Chats") { }
		public Task<List<ChatMessage>> GetByProjectIdAsync(string projectId) =>
			Collection.Find(m => m.ProjectId == projectId).ToListAsync();


    }
}
