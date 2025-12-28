using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Infrastructure.Messaging;
using TeamSync.Infrastructure.Messaging.Consumers;


public class TaskCreatedConsumer : RabbitMqConsumerBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TaskCreatedConsumer(RabbitMqSettings settings, IServiceScopeFactory scopeFactory)
        : base(settings, "teamsync.tasks.exchange", "teamsync.task.created.queue", "task.created")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(string json)
    {
        using var scope = _scopeFactory.CreateScope();

        // Resolve scoped services from the new scope
        var redis = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();
        var activityLog = scope.ServiceProvider.GetRequiredService<IActivityLogService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var projectMemberRepo = scope.ServiceProvider.GetRequiredService<IProjectMemberRepository>();
        var signalRNotificationNotifier = scope.ServiceProvider.GetRequiredService<INotificationNotifier>();

        var evt = JsonSerializer.Deserialize<TaskCreatedEvent>(json);

        await activityLog.LogAsync(
            "TASK_CREATED",
            evt.TaskId,
            $"ProjectId={evt.ProjectId}"
        );

        var members = await projectMemberRepo.GetUserIdsByProjectIdAsync(evt.ProjectId);

        foreach (var userId in members)
        {
            if (userId == evt.CreatedBy) continue;

            await notificationService.CreateAsync(
                userId,
                "New task created",
                $"Task '{evt.TaskId}' was added to the project",
                "task",
                new Dictionary<string, string>
                {
                    ["taskId"] = evt.TaskId,
                    ["projectId"] = evt.ProjectId
                }
            );
            await signalRNotificationNotifier.TaskCreatedAsync(userId, evt);
        }
    }
}
