public interface IActivityLogService
{
    Task LogAsync(string action, string entityId, string details);
}
