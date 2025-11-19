using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs;
using TeamSync.Application.DTOs.Project;
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
		private readonly IProjectMemberRepository _memberRepository;
		private readonly IProjectInvitationRepository _invRepository;

		public ProjectService(IProjectRepository projectRepository, IProjectMemberRepository memberRepository, IProjectInvitationRepository invRepository)
		{
			_projectRepository = projectRepository;
			_memberRepository = memberRepository;
			_invRepository = invRepository;
		}

		public async Task<List<ProjectResponseDto>> GetUserProjectsAsync(string userId)
		{
			var projects = await _projectRepository.GetAllByUserIdAsync(userId);

			return projects.Select(p => new ProjectResponseDto
			{
				Id = p.Id,
				Name = p.Name,
				Description = p.Description,
				OwnerId = p.OwnerId,
				CreatedAt = p.CreatedAt
			}).ToList();
		}

		public async Task<ProjectResponseDto> GetProjectByIdAsync(string id)
		{
			var project = await _projectRepository.GetByIdAsync(id)
				?? throw new NotFoundException("Project not found");

			return new ProjectResponseDto
			{
				Id = project.Id,
				Name = project.Name,
				Description = project.Description,
				OwnerId = project.OwnerId,
				CreatedAt = project.CreatedAt
			};
		}

		public async Task<ProjectResponseDto> CreateProjectAsync(string userId, CreateProjectDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ValidationException("Project name is required.");

			// 1. Create Project
			var project = new Project
			{
				Name = dto.Name,
				Description = dto.Description,
				OwnerId = userId,
				CreatedAt = DateTime.UtcNow
			};

			await _projectRepository.AddAsync(project);

			// 2. Add creator as project member (Owner role)
			var member = new ProjectMember
			{
				ProjectId = project.Id,
				UserId = userId,
				Role = ProjectRole.Owner,
				CreatedAt = DateTime.UtcNow
			};

			await _memberRepository.AddAsync(member);

			// 3. Return
			return new ProjectResponseDto
			{
				Id = project.Id,
				Name = project.Name,
				Description = project.Description,
				OwnerId = project.OwnerId,
				CreatedAt = project.CreatedAt
			};
		}


		public async Task UpdateProjectAsync(string id, UpdateProjectDto dto)
		{
			var existing = await _projectRepository.GetByIdAsync(id)
				?? throw new NotFoundException("Project not found");

			existing.Name = dto.Name;
			existing.Description = dto.Description;
			existing.UpdatedAt = DateTime.UtcNow;

			await _projectRepository.UpdateAsync(existing);
		}

		public async Task DeleteProjectAsync(string id)
		{
			bool exists = await _projectRepository.ExistsAsync(id);
			if (!exists)
				throw new NotFoundException("Project not found");

			await _projectRepository.DeleteAsync(id);
		}

		public async Task<List<ProjectInvitationDto>> GetProjectInvitationsAsync(string projectId, string userId)
		{
			var member = await _memberRepository.GetByProjectAndUserAsync(projectId, userId)
				?? throw new ForbiddenException("You are not a member of this project.");

			if (member.Role != ProjectRole.Owner && member.Role != ProjectRole.Admin)
				throw new ForbiddenException("Only Admins or Owner can view project invitations.");

			var invitations = await _invRepository.GetByProjectIdAsync(projectId);

			return invitations.Select(inv => new ProjectInvitationDto
			{
				Id = inv.Id,
				ProjectId = inv.ProjectId,
				InvitedEmail = inv.InvitedEmail,
				Role = inv.Role,
				Status = inv.Status,
				CreatedAt = inv.CreatedAt
			}).ToList();
		}

	}
}
