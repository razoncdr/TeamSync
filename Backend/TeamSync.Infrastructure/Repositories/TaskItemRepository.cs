using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;
using MongoDB.Driver;

namespace TeamSync.Infrastructure.Repositories
{
	public class TaskItemRepository : MongoRepository<TaskItem>, ITaskRepository
	{
		public TaskItemRepository(IMongoDatabase database)
			: base(database, "Tasks") { }

		public Task<bool> ExistsAsync(string id) =>
			Collection.Find(t => t.Id == id).AnyAsync();

		public Task<List<TaskItem>> GetByProjectIdAsync(string projectId) =>
			Collection.Find(t => t.ProjectId == projectId).ToListAsync();
	}
}
