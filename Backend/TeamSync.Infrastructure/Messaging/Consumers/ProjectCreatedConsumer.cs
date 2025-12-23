using System.Text.Json;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class ProjectCreatedConsumer : RabbitMqConsumerBase
{
	private readonly IRedisCacheService _redis;

	public ProjectCreatedConsumer(RabbitMqSettings settings, IRedisCacheService redis)
		: base(settings, "teamsync.tasks.exchange", "teamsync.project.created.queue", "project.created")
	{
		_redis = redis;
	}

	protected override async Task HandleMessageAsync(string json)
	{
		var evt = JsonSerializer.Deserialize<ProjectCreatedEvent>(json);

		await _redis.RemoveAsync($"user:{evt.CreatedBy}:projectIds");
		//await _mailService.sendProjectCreatedNotificationAsync(evt);

        Console.WriteLine($"[ProjectCreatedConsumer] Processed event for {evt.ProjectId}");
	}
}
