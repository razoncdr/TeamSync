using Pipelines.Sockets.Unofficial.Arenas;
using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs.Project;
using TeamSync.Application.Events;
using TeamSync.Application.Exceptions;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;
using TeamSync.Domain.Enums;

namespace TeamSync.Application.Services
{
	public class ProjectService : IProjectService
	{
		private readonly IProjectRepository _projectRepository;
		private readonly ITaskRepository _taskRepository;
		private readonly IProjectMemberRepository _memberRepository;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IEventPublisher _publisher;

		public ProjectService(
			IProjectRepository projectRepository,
			ITaskRepository taskRepository,
			IProjectMemberRepository memberRepository,
			IRedisCacheService redisCacheService, 
			IEventPublisher publisher
			)
		{
			_projectRepository = projectRepository;
			_taskRepository = taskRepository;
			_memberRepository = memberRepository;
			_redisCacheService = redisCacheService;
			_publisher = publisher;
		}

        public async Task<List<ProjectResponseDto>> GetUserProjectsAsync(string userId)
        {
            var idsCacheKey = $"user:{userId}:projectIds";
            var projectIds = await _redisCacheService.GetAsync<List<string>>(idsCacheKey);

            if (projectIds == null)
            {
                var memberships = await _memberRepository.GetAllByUserIdAsync(userId);
                if (!memberships.Any()) return new();

                projectIds = memberships.Select(m => m.ProjectId).ToList();
                await _redisCacheService.SetAsync(idsCacheKey, projectIds, TimeSpan.FromMinutes(30));
            }

            var cacheKeys = projectIds.ToDictionary(id => id, id => $"project:{id}");

            var cached = await _redisCacheService
                .GetManyAsync<ProjectResponseDto>(cacheKeys.Values);

            var missingIds = cacheKeys
                .Where(kvp => !cached.ContainsKey(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            var dbProjects = missingIds.Any()
                ? await _projectRepository.GetAllByIdsAsync(missingIds)
                : new List<Project>();

            var dbDtos = dbProjects.Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerId = p.OwnerId,
                CreatedAt = p.CreatedAt
            }).ToList();

            foreach (var dto in dbDtos)
            {
                await _redisCacheService.SetAsync($"project:{dto.Id}", dto, TimeSpan.FromMinutes(30));
            }

            return cached.Values.Concat(dbDtos).ToList();
        }

		public async Task<bool> IsUserProjectMemberAsync(string projectId, string userId)
        {
			return await _memberRepository.ExistsByUserIdAsync(projectId, userId);
        }

        private async Task EnsureProjectMemberAsync(string projectId, string userId)
        {
            var isMember = await _memberRepository.ExistsByUserIdAsync(projectId, userId);
            if (!isMember)
                throw new ForbiddenException("You are not a member of this project.");
        }


        public async Task<ProjectResponseDto> GetProjectByIdAsync(string id, string UserId)
		{
            await EnsureProjectMemberAsync(id, UserId);
            var projectCacheKey = $"project:{id}";
			var cachedProject = await _redisCacheService.GetAsync<ProjectResponseDto>(projectCacheKey);
			if (cachedProject != null) return cachedProject;

			var project = await _projectRepository.GetByIdAsync(id)
						  ?? throw new NotFoundException("Project not found");

			var dto = new ProjectResponseDto
			{
				Id = project.Id,
				Name = project.Name,
				Description = project.Description,
				OwnerId = project.OwnerId,
				CreatedAt = project.CreatedAt
			};

			await _redisCacheService.SetAsync(projectCacheKey, dto, TimeSpan.FromMinutes(30));
			return dto;
		}

		public async Task<ProjectResponseDto> CreateProjectAsync(string userId, CreateProjectDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ValidationException("Project name is required.");

			var project = new Project
			{
				Name = dto.Name,
				Description = dto.Description,
				OwnerId = userId,
				CreatedAt = DateTime.UtcNow
			};

			await _projectRepository.AddAsync(project);

			var member = new ProjectMember
			{
				ProjectId = project.Id,
				UserId = userId,
				Role = ProjectRole.Owner,
				CreatedAt = DateTime.UtcNow
			};

			await _memberRepository.AddAsync(member);

			// Publish RabbitMQ event
			await _publisher.PublishAsync(
				exchange: "teamsync.projects.exchange",
				routingKey: "project.created",
				new ProjectCreatedEvent
				{
					ProjectId = project.Id,
					Name = project.Name,
					CreatedBy = userId,
					CreatedAt = project.CreatedAt
				}
			);

			// Invalidate per-user project IDs cache
			await _redisCacheService.RemoveAsync($"user:{userId}:projectIds");

			return new ProjectResponseDto
			{
				Id = project.Id,
				Name = project.Name,
				Description = project.Description,
				OwnerId = project.OwnerId,
				CreatedAt = project.CreatedAt
			};
		}

		public async Task UpdateProjectAsync(string id, string UserId, UpdateProjectDto dto)
		{
            await EnsureProjectMemberAsync(id, UserId);
            var existing = await _projectRepository.GetByIdAsync(id)
						  ?? throw new NotFoundException("Project not found");

			existing.Name = dto.Name;
			existing.Description = dto.Description;
			existing.UpdatedAt = DateTime.UtcNow;

			await _projectRepository.UpdateAsync(existing);

			await _publisher.PublishAsync(
	exchange: "teamsync.projects.exchange",
	routingKey: "project.updated",
	new ProjectUpdatedEvent
	{
		ProjectId = existing.Id,
		NewName = dto.Name,
		NewDescription = dto.Description,
		UpdatedAt = DateTime.UtcNow
	}
);


			// Invalidate per-project cache
			await _redisCacheService.RemoveAsync($"project:{id}");

			// Invalidate all member project IDs cache
			var members = await _memberRepository.GetAllByProjectAsync(id);
			await Task.WhenAll(members.Select(m =>
				_redisCacheService.RemoveAsync($"user:{m.UserId}:projectIds")));
		}

		public async Task DeleteProjectAsync(string id, string UserId)
		{
            await EnsureProjectMemberAsync(id, UserId);
            bool exists = await _projectRepository.ExistsAsync(id);
			if (!exists)
				throw new NotFoundException("Project not found");

			var members = await _memberRepository.GetAllByProjectAsync(id);
			await Task.WhenAll(members.Select(m =>
				_redisCacheService.RemoveAsync($"user:{m.UserId}:projectIds")));

			// Remove project cache
			await _redisCacheService.RemoveAsync($"project:{id}");

			await _memberRepository.DeleteByProjectIdAsync(id);
			await _taskRepository.DeleteByProjectIdAsync(id);
			await _projectRepository.DeleteAsync(id);
		}
	}
}
