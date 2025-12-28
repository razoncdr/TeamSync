using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs;
using TeamSync.Application.Exceptions;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Enums;

namespace TeamSync.Application.Services;
public class ProjectMemberService : IProjectMemberService
{
	private readonly IProjectMemberRepository _memberRepo;
	private readonly IRedisCacheService _redisCacheService;
    private readonly IUserService _userService;

    public ProjectMemberService(IProjectMemberRepository memberRepo, IRedisCacheService redisCacheService, IUserService userService)
	{
		_memberRepo = memberRepo;
		_redisCacheService = redisCacheService;
        _userService = userService;
	}

    public async Task<List<ProjectMemberDto>> GetMembersAsync(
    string projectId,
    string currentUserId)
    {
        // Authorization
        var currentUser = await _memberRepo.GetByProjectAndUserAsync(projectId, currentUserId)
            ?? throw new ForbiddenException("You are not a member of this project.");

        // Get project members
        var members = await _memberRepo.GetAllByProjectAsync(projectId);

        // Collect unique userIds
        var userIds = members
            .Select(m => m.UserId)
            .Distinct()
            .ToList();

        // Load user basic info in ONE call
        var users = await _userService.GetByIdsAsync(userIds);

        // Build lookup dictionary
        var userMap = users.ToDictionary(u => u.Id);

        // Compose DTO
        return members.Select(m =>
        {
            userMap.TryGetValue(m.UserId, out var user);

            return new ProjectMemberDto
            {
                UserId = m.UserId,
                Email = user?.Email ?? "unknown",
                Name = user?.Name,
                Role = m.Role,
                JoinedAt = m.JoinedAt
            };
        }).ToList();
    }


    public async Task RemoveMemberAsync(string projectId, string userId, string currentUserId)
	{
		var currentUser = await _memberRepo.GetByProjectAndUserAsync(projectId, currentUserId)
			?? throw new ForbiddenException("You are not a member of this project.");

		var member = await _memberRepo.GetByProjectAndUserAsync(projectId, userId)
			?? throw new NotFoundException("Member not found.");

		if (member.Role == ProjectRole.Owner ||
		   (member.Role == ProjectRole.Admin && currentUser.Role != ProjectRole.Owner))
			throw new ForbiddenException("Cannot remove this member.");

		if (currentUser.Role != ProjectRole.Owner && currentUser.Role != ProjectRole.Admin)
			throw new ForbiddenException("Only Admins can remove members.");

		await _redisCacheService.RemoveAsync($"user:{userId}:projects");

		await _memberRepo.DeleteAsync(member.Id);
	}
}