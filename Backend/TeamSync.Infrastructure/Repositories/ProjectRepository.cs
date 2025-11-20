using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;
using MongoDB.Driver;

namespace TeamSync.Infrastructure.Repositories
{
	public class ProjectRepository : MongoRepository<Project>, IProjectRepository
	{
		public ProjectRepository(IMongoDatabase database)
			: base(database, "Projects") { }
		public Task<List<Project>> GetAllByUserIdAsync(string userId) =>
			Collection.Find(p => p.OwnerId == userId).ToListAsync();
		public async Task<List<Project>> GetAllByIdsAsync(List<string> ids)
		{
			return await Collection.Find(p => ids.Contains(p.Id)).ToListAsync();
		}

	}
}
