using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamSync.Domain.Entities
{
	public enum TaskItemStatus
	{
		Todo,
		InProgress,
		Completed,
		Paused
	}

	public class TaskItem
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; } = string.Empty;

		public string ProjectId { get; set; } = string.Empty;

		public string Title { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		// stored as UTC
		public DateTime? DueDate { get; set; }

		public List<string> AssignedMemberIds { get; set; } = new();

		public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; }
	}
}
