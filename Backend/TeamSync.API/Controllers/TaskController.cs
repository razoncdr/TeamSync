using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamSync.Application.DTOs.Task;
using TeamSync.Application.Interfaces.Services;

[ApiController]
[Route("api/projects/{projectId}/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? throw new UnauthorizedAccessException("User not found in token.");

    [HttpGet]
    public async Task<IActionResult> GetTasks(string projectId)
    {
        var tasks = await _taskService.GetTasksByProjectAsync(projectId, UserId);
        return Ok(new { success = true, data = tasks });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string projectId, string id)
    {
        var task = await _taskService.GetByIdAsync(id, projectId, UserId);
        return Ok(new { success = true, data = task });
    }

    [HttpPost]
    public async Task<IActionResult> Create(string projectId, [FromBody] CreateTaskDto dto)
    {
        var task = await _taskService.CreateTaskAsync(projectId, UserId, dto);
        return Ok(new { success = true, message = "Task created", data = task });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string projectId, string id, [FromBody] UpdateTaskDto dto)
    {
        var updated = await _taskService.UpdateTaskAsync(id, projectId, UserId, dto);
        return Ok(new { success = true, message = "Task updated", data = updated });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string projectId, string id)
    {
        await _taskService.DeleteTaskAsync(id, projectId, UserId);
        return Ok(new { success = true, message = "Task deleted" });
    }
}
