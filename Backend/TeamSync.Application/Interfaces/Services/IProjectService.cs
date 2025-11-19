using TeamSync.Application.DTOs;
using TeamSync.Application.DTOs.Project;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IProjectService
	{
		Task<List<ProjectResponseDto>> GetUserProjectsAsync(string userId);
		Task<ProjectResponseDto> GetProjectByIdAsync(string id);
		Task<List<ProjectInvitationDto>> GetProjectInvitationsAsync(string projectId, string userId);
		Task<ProjectResponseDto> CreateProjectAsync(string userId, CreateProjectDto dto);
		Task UpdateProjectAsync(string id, UpdateProjectDto dto);
		Task DeleteProjectAsync(string id);
	}
}
