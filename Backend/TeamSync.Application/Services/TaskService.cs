using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs.Task;
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

		public async Task<TaskItem> CreateTaskAsync(string projectId, CreateTaskDto dto)
		{
			var task = new TaskItem
			{
				ProjectId = projectId,
				Title = dto.Title,
				Description = dto.Description,
				DueDate = dto.DueDate,
				AssignedMemberIds = dto.AssignedMemberIds ?? new List<string>(),
				Status = dto.Status,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			await _taskRepository.AddAsync(task);
			return task;
		}

		public async Task<TaskItem> UpdateTaskAsync(string id, UpdateTaskDto dto)
		{
			var existing = await _taskRepository.GetByIdAsync(id)
				?? throw new NotFoundException("Task not found");

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
			bool exists = await _taskRepository.ExistsAsync(id);
			if (!exists)
				throw new NotFoundException("Task not found");
			await _taskRepository.DeleteAsync(id);
		}

		public async Task<TaskItem?> GetByIdAsync(string id)
		{
			var task = await _taskRepository.GetByIdAsync(id)??
				throw new NotFoundException("Task not found");
			return task;
		}
	}
}
