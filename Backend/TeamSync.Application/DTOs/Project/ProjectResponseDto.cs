namespace TeamSync.Application.DTOs.Project
{
	public class ProjectResponseDto
	{
		public string Id { get; set; } = "";
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public string OwnerId { get; set; } = "";
		public DateTime CreatedAt { get; set; }
	}
}
