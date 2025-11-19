using MongoDB.Driver;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;

namespace TeamSync.Infrastructure.Repositories
{
	public class TaskItemRepository : MongoRepository<TaskItem>, ITaskRepository
	{
		private readonly IMongoCollection<TaskItem> _tasks;

		public TaskItemRepository(IMongoDatabase database)
			: base(database, "Tasks")
		{
			_tasks = database.GetCollection<TaskItem>("Tasks");
		}
		public async Task<bool> ExistsAsync(string id)
		{
			var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, id);
			return await _tasks.Find(filter).AnyAsync();
		}
		public async Task<List<TaskItem>> GetByProjectIdAsync(string projectId) =>
			await _tasks.Find(t => t.ProjectId == projectId).ToListAsync();

		public async Task<TaskItem?> GetByIdAsync(string id) =>
			await _tasks.Find(t => t.Id == id).FirstOrDefaultAsync();

		public async Task AddAsync(TaskItem task) =>
			await _tasks.InsertOneAsync(task);

		public async Task UpdateAsync(TaskItem task) =>
			await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);

		public async Task DeleteAsync(string id) =>
			await _tasks.DeleteOneAsync(t => t.Id == id);
	}
}
