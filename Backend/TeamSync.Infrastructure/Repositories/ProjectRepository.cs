using MongoDB.Driver;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;

namespace TeamSync.Infrastructure.Repositories
{
	public class ProjectRepository : MongoRepository<Project>, IProjectRepository
	{
		private readonly IMongoCollection<Project> _projects;

		public ProjectRepository(IMongoDatabase database)
			: base(database, "Projects")
		{
			_projects = database.GetCollection<Project>("Projects");
		}

		public async Task<List<Project>> GetAllByUserIdAsync(string userId) =>
			await _projects.Find(p => p.OwnerId == userId).ToListAsync();

		public async Task<Project?> GetByIdAsync(string id) =>
			await _projects.Find(p => p.Id == id).FirstOrDefaultAsync();

		public async Task AddAsync(Project project) =>
			await _projects.InsertOneAsync(project);

		public async Task UpdateAsync(Project project) =>
			await _projects.ReplaceOneAsync(p => p.Id == project.Id, project);

		public async Task DeleteAsync(string id) =>
			await _projects.DeleteOneAsync(p => p.Id == id);
	}
}
