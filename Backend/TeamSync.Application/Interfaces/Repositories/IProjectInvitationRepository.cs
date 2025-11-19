using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IProjectInvitationRepository: IRepository<ProjectInvitation>
	{
		Task<List<ProjectInvitation>> GetByUserEmailAsync(string email);
		Task<List<ProjectInvitation>> GetByProjectIdAsync(string projectId);
		Task<bool> ExistsPendingAsync(string projectId, string email);
	}
}