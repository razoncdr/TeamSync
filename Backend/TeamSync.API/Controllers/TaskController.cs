using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class TaskController : ControllerBase
	{
		private readonly ITaskService _taskService;

		public TaskController(ITaskService taskService)
		{
			_taskService = taskService;
		}

		[HttpGet]
		public async Task<IActionResult> GetTasks([FromQuery] string projectId)
		{
			// optional: validate project membership here
			var tasks = await _taskService.GetTasksByProjectAsync(projectId);
			return Ok(tasks);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			var task = await _taskService.GetByIdAsync(id);
			if (task == null) return NotFound();
			return Ok(task);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] TaskDto dto)
		{
			// optional: check if current user is allowed to create task in project
			var task = await _taskService.CreateTaskAsync(dto);
			return Ok(task);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] TaskDto dto)
		{
			dto.Id = id;
			var updated = await _taskService.UpdateTaskAsync(dto);
			return Ok(updated);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			await _taskService.DeleteTaskAsync(id);
			return Ok(new { success = true });
		}
	}
}
