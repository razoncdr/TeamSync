using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IProjectInvitationRepository: IRepository<ProjectInvitation>
	{
		Task<List<ProjectInvitation>> GetByUserEmailAsync(string email);
		Task<bool> ExistsPendingAsync(string projectId, string email);
	}
}