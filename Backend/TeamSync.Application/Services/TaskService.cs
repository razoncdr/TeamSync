using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs.Task;
using TeamSync.Application.Events;
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
        private readonly IRedisCacheService _redis;
        private readonly IEventPublisher _publisher;


        public TaskService(ITaskRepository taskRepository, IProjectMemberRepository memberRepository, IRedisCacheService redis,
    IEventPublisher publisher)
        {
            _taskRepository = taskRepository;
            _memberRepository = memberRepository;
            _redis = redis;
            _publisher = publisher;
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
            var cacheKey = $"tasks:project:{projectId}";

            var cached = await _redis.GetAsync<List<TaskItem>>(cacheKey);
            if (cached != null)
                return cached;

            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

            await _redis.SetAsync(cacheKey, tasks, TimeSpan.FromSeconds(60));

            return tasks;
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

            await _redis.RemoveAsync($"tasks:project:{projectId}");

            await _publisher.PublishAsync(
                exchange: "teamsync.tasks.exchange",
                routingKey: "task.created",
                new TaskCreatedEvent
                {
                    TaskId = task.Id,
                    ProjectId = projectId,
                    CreatedBy = currentUserId,
                    CreatedAt = task.CreatedAt
                });
            // Notify assigned users
            foreach (var userId in task.AssignedMemberIds)
            {
                await _publisher.PublishAsync(
                    "teamsync.tasks.exchange",
                    "task.assigned",
                    new TaskAssignedEvent
                    {
                        TaskId = task.Id,
                        ProjectId = projectId,
                        AssignedToUserId = userId,
                        AssignedBy = currentUserId,
                        AssignedAt = DateTime.UtcNow
                    }
                );
            }
            return task;
        }

        public async Task<TaskItem> UpdateTaskAsync(string id, string projectId, string currentUserId, UpdateTaskDto dto)
        {
            await EnsureProjectMemberAsync(projectId, currentUserId);

            var existing = await _taskRepository.GetByIdAsync(id)
                           ?? throw new NotFoundException("Task not found");

            if (existing.ProjectId != projectId)
                throw new ForbiddenException("Task does not belong to this project.");

            var oldAssigned = existing.AssignedMemberIds.ToHashSet();
            var newAssigned = (dto.AssignedMemberIds ?? new List<string>()).ToHashSet();

            var newlyAssigned = newAssigned.Except(oldAssigned).ToList();
            var unassigned = oldAssigned.Except(newAssigned).ToList();


            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.DueDate = dto.DueDate;
            existing.AssignedMemberIds = dto.AssignedMemberIds ?? new List<string>();
            existing.Status = dto.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(existing);

            await _redis.RemoveAsync($"tasks:project:{projectId}");

            await _publisher.PublishAsync(
                exchange: "teamsync.tasks.exchange",
                routingKey: "task.updated",
                new TaskUpdatedEvent
                {
                    TaskId = existing.Id,
                    ProjectId = projectId,
                    UpdatedBy = currentUserId,
                    UpdatedAt = existing.UpdatedAt ?? DateTime.UtcNow
                });

            foreach (var userId in newlyAssigned)
            {
                await _publisher.PublishAsync(
                    "teamsync.tasks.exchange",
                    "task.assigned",
                    new TaskAssignedEvent
                    {
                        TaskId = existing.Id,
                        ProjectId = projectId,
                        AssignedToUserId = userId,
                        AssignedBy = currentUserId,
                        AssignedAt = DateTime.UtcNow
                    });
            }

            foreach (var userId in unassigned)
            {
                await _publisher.PublishAsync(
                    "teamsync.tasks.exchange",
                    "task.unassigned",
                    new TaskUnassignedEvent
                    {
                        TaskId = existing.Id,
                        ProjectId = projectId,
                        UnassignedFromUserId = userId,
                        ActionBy = currentUserId,
                        ActionAt = DateTime.UtcNow
                    });
            }

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

            await _redis.RemoveAsync($"tasks:project:{projectId}");

            await _publisher.PublishAsync(
                exchange: "teamsync.tasks.exchange",
                routingKey: "task.deleted",
                new TaskDeletedEvent
                {
                    TaskId = task.Id,
                    ProjectId = projectId,
                    DeletedBy = currentUserId
                });
        }

    }
}
