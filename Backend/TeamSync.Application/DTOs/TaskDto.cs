using TeamSync.Domain.Entities;

namespace TeamSync.Application.DTOs
{
	public class TaskDto
	{
		public string Id { get; set; } = string.Empty;
		public string ProjectId { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
		public List<string> AssignedMemberIds { get; set; } = new();
		public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
		public DateTime CreatedAt { get; set; }
	}
}
