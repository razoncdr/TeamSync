using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;
using MongoDB.Driver;

namespace TeamSync.Infrastructure.Repositories
{
	public class TaskItemRepository : MongoRepository<TaskItem>, ITaskRepository
	{
		public TaskItemRepository(IMongoDatabase database)
			: base(database, "Tasks") { }

		public Task<List<TaskItem>> GetByProjectIdAsync(string projectId) =>
			Collection.Find(t => t.ProjectId == projectId).ToListAsync();

		public async Task DeleteByProjectIdAsync(string projectId)
		{
			var filter = Builders<TaskItem>.Filter.Eq(t => t.ProjectId, projectId);
			await Collection.DeleteManyAsync(filter);
		}
	}
}
