using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TeamSync.Application.Events;

namespace TeamSync.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
	private readonly IConnection _connection;
	private readonly IChannel _channel;
	private readonly RabbitMqSettings _settings;

	public RabbitMqEventPublisher(RabbitMqSettings settings)
	{
		_settings = settings;

		var factory = new ConnectionFactory
		{
			HostName = settings.HostName,
			UserName = settings.UserName,
			Password = settings.Password
		};

		_connection = factory.CreateConnectionAsync().Result;
		_channel = _connection.CreateChannelAsync().Result;

        _channel.ExchangeDeclareAsync("teamsync.projects.exchange", "topic", durable: true).Wait();
        _channel.ExchangeDeclareAsync("teamsync.tasks.exchange", "topic", durable: true).Wait();
    }

	public async Task PublishAsync(string exchange, string routingKey, object message)
	{
		var json = JsonSerializer.Serialize(message);
		var body = Encoding.UTF8.GetBytes(json);

		await _channel.BasicPublishAsync(
			exchange: exchange,
			routingKey: routingKey,
			body: body
		);
	}

	public void Dispose()
	{
		_channel?.Dispose();
		_connection?.Dispose();
	}
}
