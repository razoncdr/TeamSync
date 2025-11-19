using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/invitations")]
	public class ProjectInvitationsController : ControllerBase
	{
		private readonly IProjectInvitationService _invitationService;

		public ProjectInvitationsController(IProjectInvitationService invitationService)
		{
			_invitationService = invitationService;
		}
		private string UserEmail =>
			User.FindFirstValue(ClaimTypes.Email)
			?? throw new UnauthorizedAccessException("User not found in token.");
		private string UserId =>
			User.FindFirstValue(ClaimTypes.NameIdentifier)
			?? throw new UnauthorizedAccessException("User not found in token.");

		[HttpGet]
		public async Task<IActionResult> GetUserInvitations()
		{
			var invitations = await _invitationService.GetUserInvitationsAsync(UserEmail);
			return Ok(new { success = true, message = "Invitations fetched", data = invitations });
		}

		[HttpPost("{invitationId}/accept")]
		public async Task<IActionResult> Accept(string invitationId)
		{
			await _invitationService.AcceptInvitationAsync(invitationId, UserId);
			return Ok(new { success = true, message = "Invitation accepted" });
		}

		[HttpPost("{invitationId}/reject")]
		public async Task<IActionResult> Reject(string invitationId)
		{
			await _invitationService.RejectInvitationAsync(invitationId, UserId);
			return Ok(new { success = true, message = "Invitation rejected" });
		}
	}
}
