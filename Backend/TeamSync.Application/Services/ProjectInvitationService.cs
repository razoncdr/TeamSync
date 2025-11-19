using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs;
using TeamSync.Application.Exceptions;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;
using TeamSync.Domain.Enums;

namespace TeamSync.Application.Services;
public class ProjectInvitationService : IProjectInvitationService
{
	private readonly IProjectInvitationRepository _invRepo;
	private readonly IProjectMemberRepository _memberRepo;

	public ProjectInvitationService(IProjectInvitationRepository invRepo, IProjectMemberRepository memberRepo)
	{
		_invRepo = invRepo;
		_memberRepo = memberRepo;
	}

	public async Task<ProjectInvitationDto> InviteAsync(string projectId, InviteMemberDto dto, string invitedByUserId)
	{
		var inviter = await _memberRepo.GetByProjectAndUserAsync(projectId, invitedByUserId)
			?? throw new ForbiddenException("You are not a member of this project.");

		if (inviter.Role != ProjectRole.Owner && inviter.Role != ProjectRole.Admin)
			throw new ForbiddenException("Only Admins can invite members.");

		if (await _invRepo.ExistsPendingAsync(projectId, dto.Email))
			throw new ConflictException("Invitation already sent.");

		if (await _memberRepo.ExistsByUserIdAsync(projectId, dto.Email))
			throw new ConflictException("User is already a member.");

		var invitation = new ProjectInvitation
		{
			ProjectId = projectId,
			InvitedEmail = dto.Email,
			InvitedByUserId = invitedByUserId,
			Role = dto.Role,
			Status = InvitationStatus.Pending
		};

		await _invRepo.AddAsync(invitation);

		return new ProjectInvitationDto
		{
			Id = invitation.Id,
			ProjectId = invitation.ProjectId,
			InvitedEmail = invitation.InvitedEmail,
			Role = invitation.Role,
			Status = invitation.Status,
			CreatedAt = invitation.CreatedAt
		};
	}

	public async Task<List<ProjectInvitationDto>> GetUserInvitationsAsync(string userEmail)
	{
		var invitations = await _invRepo.GetByUserEmailAsync(userEmail);
		return invitations.Select(inv => new ProjectInvitationDto
		{
			Id = inv.Id,
			ProjectId = inv.ProjectId,
			InvitedEmail = inv.InvitedEmail,
			Role = inv.Role,
			Status = inv.Status,
			CreatedAt = inv.CreatedAt
		}).ToList();
	}

	public async Task AcceptInvitationAsync(string invitationId, string userId)
	{
		var invitation = await _invRepo.GetByIdAsync(invitationId)
			?? throw new NotFoundException("Invitation not found.");

		if (invitation.Status != InvitationStatus.Pending)
			throw new ConflictException("Invitation already processed.");

		invitation.InvitedUserId = userId;
		invitation.Status = InvitationStatus.Accepted;
		await _invRepo.UpdateAsync(invitation);

		var member = new ProjectMember
		{
			ProjectId = invitation.ProjectId,
			UserId = userId,
			Role = invitation.Role,
			JoinedAt = DateTime.UtcNow
		};

		await _memberRepo.AddAsync(member);
	}

	public async Task RejectInvitationAsync(string invitationId, string userId)
	{
		var invitation = await _invRepo.GetByIdAsync(invitationId)
			?? throw new NotFoundException("Invitation not found.");

		if (invitation.Status != InvitationStatus.Pending)
			throw new ConflictException("Invitation already processed.");

		invitation.Status = InvitationStatus.Rejected;
		await _invRepo.UpdateAsync(invitation);
	}

	public async Task DeleteAsync(string id)
	{
		bool exists = await _invRepo.ExistsAsync(id);
		if (!exists)
			throw new NotFoundException("Invitation not found");

		await _invRepo.DeleteAsync(id);
	}
}