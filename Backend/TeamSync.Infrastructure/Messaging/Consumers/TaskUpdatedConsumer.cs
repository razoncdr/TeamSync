using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class TaskUpdatedConsumer : RabbitMqConsumerBase
{
    private readonly IRedisCacheService _redis;

    public TaskUpdatedConsumer(RabbitMqSettings settings, IRedisCacheService redis)
        : base(settings, "teamsync.tasks.exchange", "teamsync.task.updated.queue", "task.updated")
    {
        _redis = redis;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        var evt = JsonSerializer.Deserialize<TaskUpdatedEvent>(json);

        // Invalidate tasks list cache for this project
        await _redis.RemoveAsync($"tasks:project:{evt.ProjectId}");

        // Optional: send notifications
        // await _mailService.SendTaskUpdatedNotificationAsync(evt);

        Console.WriteLine($"[TaskUpdatedConsumer] Processed event for TaskId: {evt.TaskId}");
    }
}
