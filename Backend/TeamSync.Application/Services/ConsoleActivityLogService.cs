public class ConsoleActivityLogService : IActivityLogService
{
    public Task LogAsync(string action, string entityId, string details)
    {
        Console.WriteLine(
            $"[ACTIVITY] {action} | Entity: {entityId} | {details}"
        );
        return Task.CompletedTask;
    }
}
