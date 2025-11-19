using TeamSync.Domain.Enums;

namespace TeamSync.Domain.Entities
{
	public class ProjectMember : BaseEntity
	{
		public string ProjectId { get; set; } = null!;
		public string UserId { get; set; } = null!;
		public ProjectRole Role { get; set; }
		public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
	}
}