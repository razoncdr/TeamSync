namespace TeamSync.Application.Events;

public class ProjectCreatedEvent
{
	public string ProjectId { get; set; } = default!;
	public string Name { get; set; } = default!;
	public string CreatedBy { get; set; } = default!;
	public DateTime CreatedAt { get; set; }
}
