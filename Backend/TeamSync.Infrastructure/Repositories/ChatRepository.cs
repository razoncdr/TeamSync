using MongoDB.Driver;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Collections.Generic;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;

namespace TeamSync.Infrastructure.Repositories
{
	public class ChatRepository : MongoRepository<ChatMessage>, IChatRepository
    {
		public ChatRepository(IMongoDatabase database)
			: base(database, "Chats") { }
		public Task<List<ChatMessage>> GetByProjectIdAsync(string projectId) =>
			Collection.Find(m => m.ProjectId == projectId).ToListAsync();

        public Task<List<ChatMessage>> GetPaginatedByProjectIdAsync(string projectId, int skip, int limit) =>
            Collection.Find(m => m.ProjectId == projectId)
            .SortByDescending(x => x.CreatedAt) // newest first
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }
}
