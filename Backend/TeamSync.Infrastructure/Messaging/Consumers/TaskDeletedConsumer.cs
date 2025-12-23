using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class TaskDeletedConsumer : RabbitMqConsumerBase
{
    private readonly IRedisCacheService _redis;

    public TaskDeletedConsumer(RabbitMqSettings settings, IRedisCacheService redis)
        : base(settings, "teamsync.tasks.exchange", "teamsync.task.deleted.queue", "task.deleted")
    {
        _redis = redis;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        var evt = JsonSerializer.Deserialize<TaskDeletedEvent>(json);

        // Invalidate tasks list cache for this project
        await _redis.RemoveAsync($"tasks:project:{evt.ProjectId}");

        // Optional: send notifications
        // await _mailService.SendTaskDeletedNotificationAsync(evt);

        Console.WriteLine($"[TaskDeletedConsumer] Processed event for TaskId: {evt.TaskId}");
    }
}
