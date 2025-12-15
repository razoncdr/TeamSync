using TeamSync.Application.DTOs;
using TeamSync.Application.DTOs.Task;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
	public interface ITaskService
	{
		Task<List<TaskItem>> GetTasksByProjectAsync(string projectId);
		Task<TaskItem> CreateTaskAsync(string projectId, CreateTaskDto dto);
		Task<TaskItem> UpdateTaskAsync(string id, UpdateTaskDto dto);
		Task DeleteTaskAsync(string id);
		Task<TaskItem?> GetByIdAsync(string id);
	}
}
