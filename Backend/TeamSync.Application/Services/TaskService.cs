using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services
{
	public class TaskService : ITaskService
	{
		private readonly ITaskRepository _taskRepository;

		public TaskService(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public async Task<List<TaskItem>> GetTasksByProjectAsync(string projectId)
		{
			return await _taskRepository.GetByProjectIdAsync(projectId);
		}

		public async Task<TaskItem> CreateTaskAsync(TaskDto dto)
		{
			var task = new TaskItem
			{
				ProjectId = dto.ProjectId,
				Title = dto.Title,
				Description = dto.Description,
				DueDate = dto.DueDate,
				AssignedMemberIds = dto.AssignedMemberIds ?? new List<string>(),
				Status = dto.Status,
				CreatedAt = DateTime.UtcNow
			};

			await _taskRepository.AddAsync(task);
			return task;
		}

		public async Task<TaskItem> UpdateTaskAsync(TaskDto dto)
		{
			var existing = await _taskRepository.GetByIdAsync(dto.Id);
			if (existing == null) throw new Exception("Task not found");

			existing.Title = dto.Title;
			existing.Description = dto.Description;
			existing.DueDate = dto.DueDate;
			existing.AssignedMemberIds = dto.AssignedMemberIds ?? new List<string>();
			existing.Status = dto.Status;
			existing.UpdatedAt = DateTime.UtcNow;

			await _taskRepository.UpdateAsync(existing);
			return existing;
		}

		public async Task DeleteTaskAsync(string id)
		{
			await _taskRepository.DeleteAsync(id);
		}

		public async Task<TaskItem?> GetByIdAsync(string id)
		{
			return await _taskRepository.GetByIdAsync(id);
		}
	}
}
