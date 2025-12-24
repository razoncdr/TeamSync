using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class TaskCreatedConsumer : RabbitMqConsumerBase
{
    private readonly IRedisCacheService _redis;
    private readonly IActivityLogService _activityLog;

    public TaskCreatedConsumer(RabbitMqSettings settings, IRedisCacheService redis, IActivityLogService activityLogService)
        : base(settings, "teamsync.tasks.exchange", "teamsync.task.created.queue", "task.created")
    {
        _redis = redis;
        _activityLog = activityLogService;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        var evt = JsonSerializer.Deserialize<TaskCreatedEvent>(json);

        await _activityLog.LogAsync(
            "TASK_CREATED",
            evt.TaskId,
            $"ProjectId={evt.ProjectId}"
        );

    }
}
