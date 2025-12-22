using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs.Task;
using TeamSync.Application.Exceptions;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectMemberRepository _memberRepository;

        public TaskService(ITaskRepository taskRepository, IProjectMemberRepository memberRepository)
        {
            _taskRepository = taskRepository;
            _memberRepository = memberRepository;
        }

        private async Task EnsureProjectMemberAsync(string projectId, string userId)
        {
            var isMember = await _memberRepository.ExistsByUserIdAsync(projectId, userId);
            if (!isMember)
                throw new ForbiddenException("You are not a member of this project.");
        }

        public async Task<List<TaskItem>> GetTasksByProjectAsync(string projectId, string currentUserId)
        {
            await EnsureProjectMemberAsync(projectId, currentUserId);
            return await _taskRepository.GetByProjectIdAsync(projectId);
        }

        public async Task<TaskItem> GetByIdAsync(string id, string projectId, string currentUserId)
        {
            await EnsureProjectMemberAsync(projectId, currentUserId);

            var task = await _taskRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException("Task not found");

            if (task.ProjectId != projectId)
                throw new ForbiddenException("Task does not belong to this project.");

            return task;
        }

        public async Task<TaskItem> CreateTaskAsync(string projectId, string currentUserId, CreateTaskDto dto)
        {
            await EnsureProjectMemberAsync(projectId, currentUserId);

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

        public async Task<TaskItem> UpdateTaskAsync(string id, string projectId, string currentUserId, UpdateTaskDto dto)
        {
            await EnsureProjectMemberAsync(projectId, currentUserId);

            var existing = await _taskRepository.GetByIdAsync(id)
                           ?? throw new NotFoundException("Task not found");

            if (existing.ProjectId != projectId)
                throw new ForbiddenException("Task does not belong to this project.");

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.DueDate = dto.DueDate;
            existing.AssignedMemberIds = dto.AssignedMemberIds ?? new List<string>();
            existing.Status = dto.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(existing);
            return existing;
        }

        public async Task DeleteTaskAsync(string id, string projectId, string currentUserId)
        {
            await EnsureProjectMemberAsync(projectId, currentUserId);

            var task = await _taskRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException("Task not found");

            if (task.ProjectId != projectId)
                throw new ForbiddenException("Task does not belong to this project.");

            await _taskRepository.DeleteAsync(id);
        }
    }
}
