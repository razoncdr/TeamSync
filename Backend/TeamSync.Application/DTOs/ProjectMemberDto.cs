using TeamSync.Domain.Enums;

namespace TeamSync.Application.DTOs
{
	public class ProjectMemberDto
	{
		public string UserId { get; set; } = null!;
        public string Email { get; set; } = default!;
        public string? Name { get; set; }
        public ProjectRole Role { get; set; }
		public DateTime JoinedAt { get; set; }
	}
}