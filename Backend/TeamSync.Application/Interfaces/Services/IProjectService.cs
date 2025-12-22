using TeamSync.Application.DTOs;
using TeamSync.Application.DTOs.Project;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IProjectService
	{
		Task<List<ProjectResponseDto>> GetUserProjectsAsync(string userId);
		Task<bool> IsUserProjectMemberAsync(string projectId, string userId);
        Task<ProjectResponseDto> GetProjectByIdAsync(string id, string UserId);
		Task<ProjectResponseDto> CreateProjectAsync(string userId, CreateProjectDto dto);
		Task UpdateProjectAsync(string id, string UserId, UpdateProjectDto dto);
		Task DeleteProjectAsync(string id, string UserId);
	}
}
