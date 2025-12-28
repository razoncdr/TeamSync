using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Infrastructure.Messaging;
using TeamSync.Infrastructure.Messaging.Consumers;
using TeamSync.Application.Events;

public class TaskUnassignedConsumer : RabbitMqConsumerBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TaskUnassignedConsumer(RabbitMqSettings settings, IServiceScopeFactory scopeFactory)
        : base(settings,
              "teamsync.tasks.exchange",
              "teamsync.task.unassigned.queue",
              "task.unassigned")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        using var scope = _scopeFactory.CreateScope();

        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var notifier = scope.ServiceProvider.GetRequiredService<INotificationNotifier>();
        var activityLog = scope.ServiceProvider.GetRequiredService<IActivityLogService>();

        var evt = JsonSerializer.Deserialize<TaskUnassignedEvent>(json)!;

        await activityLog.LogAsync(
            "TASK_UNASSIGNED",
            evt.TaskId,
            $"User={evt.UnassignedFromUserId}"
        );

        await notificationService.CreateAsync(
            evt.UnassignedFromUserId,
            "Task unassigned",
            "You were removed from a task",
            "task",
            new Dictionary<string, string>
            {
                ["taskId"] = evt.TaskId,
                ["projectId"] = evt.ProjectId
            }
        );

        await notifier.TaskUnassignedAsync(evt.UnassignedFromUserId, evt);
    }
}
