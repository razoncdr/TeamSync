using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IProjectMemberRepository : IRepository<ProjectMember>
	{
		Task<ProjectMember?> GetByProjectAndUserAsync(string projectId, string userId);
		Task<List<ProjectMember>> GetAllByProjectAsync(string projectId);
		Task<List<ProjectMember>> GetAllByUserIdAsync(string userId);
		Task<bool> ExistsByUserIdAsync(string projectId, string userId);
		Task DeleteByProjectIdAsync(string projectId);
	}
}