using TeamSync.Domain.Entities;
using MongoDB.Driver;
using TeamSync.Application.Interfaces.Repositories;

namespace TeamSync.Infrastructure.Repositories
{
	public class ProjectMemberRepository : MongoRepository<ProjectMember>, IProjectMemberRepository
	{
		public ProjectMemberRepository(IMongoDatabase database)
			: base(database, "ProjectMembers") { }

		public Task<ProjectMember?> GetByProjectAndUserAsync(string projectId, string userId) =>
			Collection.Find(m => m.ProjectId == projectId && m.UserId == userId).FirstOrDefaultAsync();

		public Task<List<ProjectMember>> GetAllByProjectAsync(string projectId) =>
			Collection.Find(m => m.ProjectId == projectId).ToListAsync();

		public Task<List<ProjectMember>> GetAllByUserIdAsync(string userId) =>
			Collection.Find(m => m.UserId == userId).ToListAsync();

		public Task<bool> ExistsByUserIdAsync(string projectId, string userId) =>
			Collection.Find(m => m.ProjectId == projectId && m.UserId == userId).AnyAsync();

		public async Task DeleteByProjectIdAsync(string projectId)
		{
			var filter = Builders<ProjectMember>.Filter.Eq(m => m.ProjectId, projectId);
			await Collection.DeleteManyAsync(filter);
		}
	}
}