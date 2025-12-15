using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IProjectRepository : IRepository<Project>
	{
		Task<List<Project>> GetAllByUserIdAsync(string userId);
		Task<List<Project>> GetAllByIdsAsync(List<string> ids);
	}	
}
