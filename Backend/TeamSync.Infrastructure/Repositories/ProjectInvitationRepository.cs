using TeamSync.Domain.Entities;
using TeamSync.Domain.Enums;
using MongoDB.Driver;
using TeamSync.Application.Interfaces.Repositories;

namespace TeamSync.Infrastructure.Repositories
{
	public class ProjectInvitationRepository : MongoRepository<ProjectInvitation>, IProjectInvitationRepository
	{
		public ProjectInvitationRepository(IMongoDatabase database)
			: base(database, "ProjectInvitations") { }

		public Task<ProjectInvitation?> GetByIdAsync(string id) =>
			Collection.Find(i => i.Id == id).FirstOrDefaultAsync();

		public Task<List<ProjectInvitation>> GetByUserEmailAsync(string email) =>
			Collection.Find(i => i.InvitedEmail == email).ToListAsync();

		public Task<bool> ExistsPendingAsync(string projectId, string email) =>
			Collection.Find(i => i.ProjectId == projectId && i.InvitedEmail == email && i.Status == InvitationStatus.Pending)
					  .AnyAsync();
	}
}