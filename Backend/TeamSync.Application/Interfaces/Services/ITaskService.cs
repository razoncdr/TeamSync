using TeamSync.Application.DTOs;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
	public interface ITaskService
	{
		Task<List<TaskItem>> GetTasksByProjectAsync(string projectId);
		Task<TaskItem> CreateTaskAsync(TaskDto dto);
		Task<TaskItem> UpdateTaskAsync(TaskDto dto);
		Task DeleteTaskAsync(string id);
		Task<TaskItem?> GetByIdAsync(string id);
	}
}
