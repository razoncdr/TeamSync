using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class TaskUpdatedConsumer : RabbitMqConsumerBase
{
    private readonly IRedisCacheService _redis;
    private readonly IActivityLogService _activityLog;

    public TaskUpdatedConsumer(RabbitMqSettings settings, IRedisCacheService redis, IActivityLogService activityLog)
        : base(settings, "teamsync.tasks.exchange", "teamsync.task.updated.queue", "task.updated")
    {
        _redis = redis;
        _activityLog = activityLog;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        var evt = JsonSerializer.Deserialize<TaskUpdatedEvent>(json);

        await _activityLog.LogAsync(
            "TASK_UPDATED",
            evt.TaskId,
            $"ProjectId={evt.ProjectId}"
        );
    }
}
