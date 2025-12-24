using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class TaskDeletedConsumer : RabbitMqConsumerBase
{
    private readonly IRedisCacheService _redis;
    private readonly IActivityLogService _activityLogService;
    public TaskDeletedConsumer(RabbitMqSettings settings, IRedisCacheService redis, IActivityLogService activityLogService)
        : base(settings, "teamsync.tasks.exchange", "teamsync.task.deleted.queue", "task.deleted")
    {
        _redis = redis;
        _activityLogService = activityLogService;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        var evt = JsonSerializer.Deserialize<TaskDeletedEvent>(json);

        await _activityLogService.LogAsync(
            "TASK_DELETED",
            evt.TaskId,
            $"ProjectId={evt.ProjectId}"
        );

    }
}
