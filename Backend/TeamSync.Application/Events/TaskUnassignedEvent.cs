namespace TeamSync.Application.Events
{
    public class TaskUnassignedEvent
    {
        public string TaskId { get; set; } = default!;
        public string ProjectId { get; set; } = default!;
        public string UnassignedFromUserId { get; set; } = default!;
        public string ActionBy { get; set; } = default!;
        public DateTime ActionAt { get; set; }
    }
}
