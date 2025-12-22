using TeamSync.Application.DTOs.Task;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
	public interface ITaskService
	{
		Task<List<TaskItem>> GetTasksByProjectAsync(string projectId, string currentUserId);
		Task<TaskItem> CreateTaskAsync(string projectId, string currentUserId, CreateTaskDto dto);
		Task<TaskItem> UpdateTaskAsync(string id, string projectId, string currentUserId, UpdateTaskDto dto);
		Task DeleteTaskAsync(string id, string projectId, string currentUserId);
		Task<TaskItem> GetByIdAsync(string id, string projectId, string currentUserId);
	}
}
