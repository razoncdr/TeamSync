using TeamSync.Application.DTOs;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IProjectInvitationService
	{
		Task<ProjectInvitationDto> InviteAsync(string projectId, InviteMemberDto dto, string invitedByUserId);
		Task<List<ProjectInvitationDto>> GetProjectInvitationsAsync(string projectId, string userId);
		Task<List<ProjectInvitationDto>> GetUserInvitationsAsync(string userEmail);
		Task AcceptInvitationAsync(string invitationId, string userId, string userEmail);
		Task RejectInvitationAsync(string invitationId, string userId, string userEmail);
        Task DeleteAsync(string id, string userId);
	}
}