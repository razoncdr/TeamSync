using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Infrastructure.Messaging;
using TeamSync.Infrastructure.Messaging.Consumers;
using TeamSync.Application.Events;

public class TaskAssignedConsumer : RabbitMqConsumerBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TaskAssignedConsumer(RabbitMqSettings settings, IServiceScopeFactory scopeFactory)
        : base(settings,
              "teamsync.tasks.exchange",
              "teamsync.task.assigned.queue",
              "task.assigned")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        using var scope = _scopeFactory.CreateScope();

        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var notifier = scope.ServiceProvider.GetRequiredService<INotificationNotifier>();
        var activityLog = scope.ServiceProvider.GetRequiredService<IActivityLogService>();

        var evt = JsonSerializer.Deserialize<TaskAssignedEvent>(json)!;

        await activityLog.LogAsync(
            "TASK_ASSIGNED",
            evt.TaskId,
            $"AssignedTo={evt.AssignedToUserId}"
        );

        await notificationService.CreateAsync(
            evt.AssignedToUserId,
            "Task assigned",
            "You were assigned to a task",
            "task",
            new Dictionary<string, string>
            {
                ["taskId"] = evt.TaskId,
                ["projectId"] = evt.ProjectId
            }
        );

        await notifier.TaskAssignedAsync(evt.AssignedToUserId, evt);
    }
}
