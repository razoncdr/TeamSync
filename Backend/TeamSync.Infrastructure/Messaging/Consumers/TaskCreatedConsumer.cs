using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class TaskCreatedConsumer : RabbitMqConsumerBase
{
    private readonly IRedisCacheService _redis;

    public TaskCreatedConsumer(RabbitMqSettings settings, IRedisCacheService redis)
        : base(settings, "teamsync.tasks.exchange", "teamsync.task.created.queue", "task.created")
    {
        _redis = redis;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        var evt = JsonSerializer.Deserialize<TaskCreatedEvent>(json);

        // Invalidate tasks list cache for this project
        await _redis.RemoveAsync($"tasks:project:{evt.ProjectId}");

        // Optionally, send notifications/email here
        // await _mailService.SendTaskCreatedNotificationAsync(evt);

        Console.WriteLine($"[TaskCreatedConsumer] Processed event for TaskId: {evt.TaskId}");
    }
}
