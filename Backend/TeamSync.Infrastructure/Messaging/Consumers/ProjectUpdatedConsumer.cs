using System.Text.Json;
using TeamSync.Application.Events;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public class ProjectUpdatedConsumer : RabbitMqConsumerBase
{
	public ProjectUpdatedConsumer(RabbitMqSettings settings)
		: base(settings, "teamsync.project.updated.queue", "project.updated")
	{
	}

	protected override Task HandleMessageAsync(string json)
	{
		var evt = JsonSerializer.Deserialize<ProjectUpdatedEvent>(json);

		Console.WriteLine($"[ProjectUpdatedConsumer] Project updated → {evt.ProjectId}");

		// Future work: send notifications, update analytics, etc.

		return Task.CompletedTask;
	}
}
