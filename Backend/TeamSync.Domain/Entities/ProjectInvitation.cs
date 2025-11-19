using TeamSync.Domain.Enums;

namespace TeamSync.Domain.Entities
{
	public class ProjectInvitation : BaseEntity
	{
		public string ProjectId { get; set; } = null!;
		public string InvitedEmail { get; set; } = null!;
		public string? InvitedUserId { get; set; }
		public string InvitedByUserId { get; set; } = null!;
		public ProjectRole Role { get; set; }
		public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
	}
}