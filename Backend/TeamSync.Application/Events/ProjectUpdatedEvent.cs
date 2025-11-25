namespace TeamSync.Application.Events;

public class ProjectUpdatedEvent
{
	public string ProjectId { get; set; }
	public string UpdatedBy { get; set; }
	public string NewName { get; set; }
	public string NewDescription { get; set; }
	public DateTime UpdatedAt { get; set; }
}

