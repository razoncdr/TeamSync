using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services
{
	public class ProjectService : IProjectService
	{
		private readonly IProjectRepository _projectRepository;

		public ProjectService(IProjectRepository projectRepository)
		{
			_projectRepository = projectRepository;
		}

		public async Task<List<Project>> GetUserProjectsAsync(string userId)
		{
			return await _projectRepository.GetAllByUserIdAsync(userId);
		}

		public async Task<Project> CreateProjectAsync(string userId, string name, string description)
		{
			var project = new Project
			{
				Name = name,
				Description = description,
				OwnerId = userId,
				CreatedAt = DateTime.UtcNow
			};

			await _projectRepository.AddAsync(project);
			return project;
		}

		public async Task UpdateProjectAsync(string id, string name, string description)
		{
			var existing = await _projectRepository.GetByIdAsync(id);
			if (existing == null) throw new Exception("Project not found");

			existing.Name = name;
			existing.Description = description;

			await _projectRepository.UpdateAsync(existing);
		}

		public async Task DeleteProjectAsync(string id)
		{
			await _projectRepository.DeleteAsync(id);
		}
	}
}
