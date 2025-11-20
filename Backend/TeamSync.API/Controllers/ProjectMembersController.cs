using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/projects/{projectId}/members")]
	public class ProjectMembersController : ControllerBase
	{
		private readonly IProjectMemberService _memberService;
		private readonly IProjectInvitationService _invitationService;

		public ProjectMembersController(IProjectMemberService memberService, IProjectInvitationService projectInvitationService)
		{
			_memberService = memberService;
			_invitationService = projectInvitationService;
		}
		private string UserId =>
			User.FindFirstValue(ClaimTypes.NameIdentifier)
			?? throw new UnauthorizedAccessException("User not found in token.");

		[HttpGet]
		public async Task<IActionResult> GetAll(string projectId)
		{
			var members = await _memberService.GetMembersAsync(projectId);
			return Ok(new { success = true, message = "Members fetched", data = members });
		}

		[HttpPost("invite")]
		public async Task<IActionResult> Invite(string projectId, [FromBody] InviteMemberDto dto)
		{
			var invitation = await _invitationService.InviteAsync(projectId, dto, UserId);

			return Ok(new { success = true, message = "Invitation sent", data = invitation });
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> Remove(string projectId, string userId)
		{
			await _memberService.RemoveMemberAsync(projectId, userId, UserId);
			return Ok(new { success = true, message = "Member removed" });
		}
	}
}
