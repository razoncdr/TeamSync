using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamSync.Application.DTOs.Task;
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
			return Ok(new { success = true, data = tasks });
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			var task = await _taskService.GetByIdAsync(id);
			return Ok(new { success = true, data = task });
		}

		[HttpPost("{projectId}")]
		public async Task<IActionResult> Create(string projectId, [FromBody] CreateTaskDto dto)
		{
			// optional: check if current user is allowed to create task in project
			var task = await _taskService.CreateTaskAsync(projectId, dto);
			return Ok(new { success = true, message = "Task created", data = task });
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] UpdateTaskDto dto)
		{
			var updated = await _taskService.UpdateTaskAsync(id, dto);
			return Ok(new { success = true, message = "Project updated" });
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			await _taskService.DeleteTaskAsync(id);
			return Ok(new { success = true, message = "Task deleted" });
		}
	}
}
