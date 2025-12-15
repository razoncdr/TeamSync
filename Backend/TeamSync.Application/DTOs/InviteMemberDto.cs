using TeamSync.Domain.Enums;

namespace TeamSync.Application.DTOs
{
	public class InviteMemberDto
	{
		public string Email { get; set; } = null!;
		public ProjectRole Role { get; set; }
	}
}