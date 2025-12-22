using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamSync.Application.DTOs.Project;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ProjectController : ControllerBase
	{
		private readonly IProjectService _projectService;

		public ProjectController(IProjectService projectService)
		{
			_projectService = projectService;
		}

		private string UserId =>
			User.FindFirstValue(ClaimTypes.NameIdentifier)
			?? throw new UnauthorizedAccessException("User not found in token.");

		[HttpGet]
		public async Task<IActionResult> GetProjects()
		{
			var projects = await _projectService.GetUserProjectsAsync(UserId);
			return Ok(new { success = true, data = projects });
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			var project = await _projectService.GetProjectByIdAsync(id);
			return Ok(new { success = true, data = project });
		}

		[HttpGet("{id}/is-member")]
		public async Task<IActionResult> IsMember(string id)
		{
			var isMember = await _projectService.IsUserProjectMemberAsync(id, UserId);
			return Ok(new { success = true, data = isMember });
        }

        [HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
		{
			var created = await _projectService.CreateProjectAsync(UserId, dto);
			return Ok(new { success = true, message = "Project created", data = created });
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] UpdateProjectDto dto)
		{
			await _projectService.UpdateProjectAsync(id, dto);
			return Ok(new { success = true, message = "Project updated" });
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			await _projectService.DeleteProjectAsync(id);
			return Ok(new { success = true, message = "Project deleted" });
		}
	}
}
