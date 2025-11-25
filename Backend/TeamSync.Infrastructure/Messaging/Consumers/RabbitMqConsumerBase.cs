using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace TeamSync.Infrastructure.Messaging.Consumers;

public abstract class RabbitMqConsumerBase : BackgroundService
{
	protected readonly IConnection _connection;
	protected readonly IChannel _channel;
	protected readonly string _queueName;
	protected readonly string _routingKey;
	protected readonly string _exchange;

	protected RabbitMqConsumerBase(RabbitMqSettings settings, string queueName, string routingKey)
	{
		_exchange = settings.Exchange;
		_queueName = queueName;
		_routingKey = routingKey;

		var factory = new ConnectionFactory
		{
			HostName = settings.HostName,
			UserName = settings.UserName,
			Password = settings.Password
		};

		_connection = factory.CreateConnectionAsync().Result;
		_channel = _connection.CreateChannelAsync().Result;

		// Declare queue + binding
		_channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
		_channel.QueueBindAsync(queueName, settings.Exchange, routingKey);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.ReceivedAsync += async (_, ea) =>
		{
			var message = Encoding.UTF8.GetString(ea.Body.ToArray());
			await HandleMessageAsync(message);
		};

		await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer);
	}

	protected abstract Task HandleMessageAsync(string json);
}
