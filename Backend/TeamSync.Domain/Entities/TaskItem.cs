namespace TeamSync.Domain.Entities
{
	public enum TaskItemStatus
	{
		Todo,
		InProgress,
		Completed,
		Paused
	}

	public class TaskItem : BaseEntity
	{
		public string ProjectId { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
		public List<string> AssignedMemberIds { get; set; } = new();
		public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
	}
}