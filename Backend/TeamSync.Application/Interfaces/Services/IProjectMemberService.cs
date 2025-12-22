using TeamSync.Application.DTOs;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IProjectMemberService
	{
		Task<List<ProjectMemberDto>> GetMembersAsync(string projectId, string currentUserId);
		Task RemoveMemberAsync(string projectId, string userId, string currentUserId);
	}
}