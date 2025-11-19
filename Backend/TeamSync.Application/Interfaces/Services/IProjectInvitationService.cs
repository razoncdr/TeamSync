using TeamSync.Application.DTOs;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IProjectInvitationService
	{
		Task<ProjectInvitationDto> InviteAsync(string projectId, InviteMemberDto dto, string invitedByUserId);
		Task<List<ProjectInvitationDto>> GetUserInvitationsAsync(string userEmail);
		Task AcceptInvitationAsync(string invitationId, string userId);
		Task RejectInvitationAsync(string invitationId, string userId);
	}
}