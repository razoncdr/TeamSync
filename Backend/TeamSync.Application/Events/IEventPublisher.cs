namespace TeamSync.Application.Events;

public interface IEventPublisher
{
	Task PublishAsync(string exchange, string routingKey, object message);
}