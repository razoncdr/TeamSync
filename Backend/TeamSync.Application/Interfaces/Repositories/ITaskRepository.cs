using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface ITaskRepository : IRepository<TaskItem>
	{
		Task<List<TaskItem>> GetByProjectIdAsync(string projectId);
	}
}
