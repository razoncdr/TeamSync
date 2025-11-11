using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IProjectRepository
	{
		Task<List<Project>> GetAllByUserIdAsync(string userId);
		Task<Project?> GetByIdAsync(string id);
		Task AddAsync(Project project);
		Task UpdateAsync(Project project);
		Task DeleteAsync(string id);
	}
}
