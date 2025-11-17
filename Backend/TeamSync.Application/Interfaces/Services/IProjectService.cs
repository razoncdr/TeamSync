using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IProjectService
	{
		Task<List<Project>> GetUserProjectsAsync(string userId);
		Task<Project> GetProjectByIdAsync(string projectId);
		Task<Project> CreateProjectAsync(string userId, string name, string description);
		Task UpdateProjectAsync(string id, string name, string description);
		Task DeleteProjectAsync(string id);
	}
}
