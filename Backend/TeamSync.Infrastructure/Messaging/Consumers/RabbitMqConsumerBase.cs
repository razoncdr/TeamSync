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

    protected RabbitMqConsumerBase(
        RabbitMqSettings settings,
        string exchange,
        string queueName,
        string routingKey)
    {
        _exchange = exchange;
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

        _channel.ExchangeDeclareAsync(
            exchange: _exchange,
            type: ExchangeType.Topic,
            durable: true
        ).Wait();

        _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        ).Wait();

        _channel.QueueBindAsync(
            queue: _queueName,
            exchange: _exchange,
            routingKey: _routingKey
        ).Wait();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                await HandleMessageAsync(message);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch
            {
                // no ack → retry
            }
        };

        _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer
        );

        return Task.CompletedTask;
    }

    protected abstract Task HandleMessageAsync(string json);
}
