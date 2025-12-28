namespace TeamSync.Application.Events
{
    public class TaskAssignedEvent
    {
        public string TaskId { get; set; } = default!;
        public string ProjectId { get; set; } = default!;
        public string AssignedToUserId { get; set; } = default!;
        public string AssignedBy { get; set; } = default!;
        public DateTime AssignedAt { get; set; }
    }
}
