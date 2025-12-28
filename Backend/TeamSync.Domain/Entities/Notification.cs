namespace TeamSync.Domain.Entities;

public class Notification : BaseEntity
{
    public string UserId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string Type { get; set; } = "system"; // task, project, system
    public bool IsRead { get; set; } = false;

    public Dictionary<string, string>? Metadata { get; set; }
}
