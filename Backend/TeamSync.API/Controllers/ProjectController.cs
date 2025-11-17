using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

		private string GetUserId() =>
			User.FindFirstValue(ClaimTypes.NameIdentifier) ??
			User.FindFirstValue(ClaimTypes.Name) ??
			throw new Exception("User not found in token");

		[HttpGet]
		public async Task<IActionResult> GetProjects()
		{
			var userId = GetUserId();
			var projects = await _projectService.GetUserProjectsAsync(userId);
			return Ok(projects);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetProjectById(string id)
		{
			var project = await _projectService.GetProjectByIdAsync(id);
			return Ok(project);
		}

		[HttpPost]
		public async Task<IActionResult> CreateProject([FromBody] ProjectDto dto)
		{
			var userId = GetUserId();
			var project = await _projectService.CreateProjectAsync(userId, dto.Name, dto.Description);
			return Ok(project);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProject(string id, [FromBody] ProjectDto dto)
		{
			await _projectService.UpdateProjectAsync(id, dto.Name, dto.Description);
			return Ok(new { success = true });
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProject(string id)
		{
			await _projectService.DeleteProjectAsync(id);
			return Ok(new { success = true });
		}
	}

	public class ProjectDto
	{
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}
