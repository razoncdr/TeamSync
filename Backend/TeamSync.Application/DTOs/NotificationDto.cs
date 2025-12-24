namespace TeamSync.Application.DTOs;

public class NotificationDto
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
