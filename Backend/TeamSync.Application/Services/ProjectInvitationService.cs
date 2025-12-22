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
	private readonly IRedisCacheService _redisCacheService;

	public ProjectInvitationService(IProjectInvitationRepository invRepo, IProjectMemberRepository memberRepo, IRedisCacheService redisCacheService)
	{
		_invRepo = invRepo;
		_memberRepo = memberRepo;
		_redisCacheService = redisCacheService;
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

		await _redisCacheService.RemoveAsync($"user:{dto.Email}:invitations");

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

	public async Task<List<ProjectInvitationDto>> GetProjectInvitationsAsync(string projectId, string userId)
	{
		var member = await _memberRepo.GetByProjectAndUserAsync(projectId, userId)
			?? throw new ForbiddenException("You are not a member of this project.");

		//if (member.Role != ProjectRole.Owner && member.Role != ProjectRole.Admin)
		//	throw new ForbiddenException("Only Admins or Owner can view project invitations.");

		var invitations = await _invRepo.GetByProjectIdAsync(projectId);

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
	public async Task<List<ProjectInvitationDto>> GetUserInvitationsAsync(string userEmail)
	{
		var cacheKey = $"user:{userEmail}:invitations";

		var cachedInvites = await _redisCacheService.GetAsync<List<ProjectInvitationDto>>(cacheKey);
		if (cachedInvites != null)
			return cachedInvites;

		var invitations = await _invRepo.GetByUserEmailAsync(userEmail);
		var dtos = invitations.Select(inv => new ProjectInvitationDto
		{
			Id = inv.Id,
			ProjectId = inv.ProjectId,
			InvitedEmail = inv.InvitedEmail,
			Role = inv.Role,
			Status = inv.Status,
			CreatedAt = inv.CreatedAt
		}).ToList();

		await _redisCacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10));

		return dtos;
	}

	public async Task AcceptInvitationAsync(string invitationId, string userId, string userEmail)
	{
		var invitation = await _invRepo.GetByIdAsync(invitationId)
			?? throw new NotFoundException("Invitation not found.");

		if (invitation.Status != InvitationStatus.Pending)
			throw new ConflictException("Invitation already processed.");

		if(await _memberRepo.ExistsByUserIdAsync(invitation.ProjectId, userId))
			throw new ConflictException("You are already a member of this project.");

        if (!string.Equals(invitation.InvitedEmail, userEmail, StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("This invitation was not sent to you.");


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

		await _redisCacheService.RemoveAsync($"user:{invitation.InvitedEmail}:invitations");
		await _redisCacheService.RemoveAsync($"user:{userId}:projectIds");
	}

	public async Task RejectInvitationAsync(string invitationId, string userId, string userEmail)
	{
		var invitation = await _invRepo.GetByIdAsync(invitationId)
			?? throw new NotFoundException("Invitation not found.");

		if (invitation.Status != InvitationStatus.Pending)
			throw new ConflictException("Invitation already processed.");

        if (!string.Equals(invitation.InvitedEmail, userEmail, StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("This invitation was not sent to you.");

        invitation.Status = InvitationStatus.Rejected;
		await _invRepo.UpdateAsync(invitation);

		await _redisCacheService.RemoveAsync($"user:{invitation.InvitedEmail}:invitations");
	}

	public async Task DeleteAsync(string id, string userId)
	{

		var invitation = await _invRepo.GetByIdAsync(id)
	?? throw new NotFoundException("Invitation not found");

        var member = await _memberRepo.GetByProjectAndUserAsync(invitation.ProjectId, userId)
        ?? throw new ForbiddenException("You are not a member of this project.");

        if (member.Role != ProjectRole.Owner &&
            member.Role != ProjectRole.Admin &&
            invitation.InvitedByUserId != userId)
            throw new ForbiddenException("You are not allowed to delete this invitation.");

        await _invRepo.DeleteAsync(id);

		await _redisCacheService.RemoveAsync($"user:{invitation.InvitedEmail}:invitations");
	}
}