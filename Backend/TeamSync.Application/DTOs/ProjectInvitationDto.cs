using TeamSync.Domain.Enums;

namespace TeamSync.Application.DTOs
{
	public class ProjectInvitationDto
	{
		public string Id { get; set; } = null!;
		public string ProjectId { get; set; } = null!;
		public string InvitedEmail { get; set; } = null!;
		public string? InvitedUserId { get; set; }
		public ProjectRole Role { get; set; }
		public InvitationStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}