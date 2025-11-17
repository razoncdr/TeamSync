using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface ITaskRepository
	{
		Task<List<TaskItem>> GetByProjectIdAsync(string projectId);
		Task<TaskItem?> GetByIdAsync(string id);
		Task AddAsync(TaskItem task);
		Task UpdateAsync(TaskItem task);
		Task DeleteAsync(string id);
	}
}
