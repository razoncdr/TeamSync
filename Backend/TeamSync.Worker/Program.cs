using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using TeamSync.Application.Events;

class Program
{
	static async Task Main(string[] args)
	{
		var factory = new ConnectionFactory
		{
			HostName = "localhost",
			UserName = "guest",
			Password = "guest"
		};

		using var connection = await factory.CreateConnectionAsync();
		using var channel = await connection.CreateChannelAsync();

		string exchange = "teamsync.projects.exchange";
		string queueName = "teamsync.project.events.queue";

		await channel.ExchangeDeclareAsync(exchange, "topic", durable: true);

		await channel.QueueDeclareAsync(
			queue: queueName,
			durable: true,
			exclusive: false,
			autoDelete: false
		);

		await channel.QueueBindAsync(queueName, exchange, "project.created");

		var consumer = new AsyncEventingBasicConsumer(channel);

		consumer.ReceivedAsync += async (_, ea) =>
		{
			var json = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
			var data = JsonSerializer.Deserialize<ProjectCreatedEvent>(json);

			Console.WriteLine($"📩 PROJECT CREATED EVENT RECEIVED: {data.Name}");

			await Task.CompletedTask;
		};

		await channel.BasicConsumeAsync(queueName, autoAck: true, consumer);

		Console.WriteLine("Worker running. Press enter to exit...");
		Console.ReadLine();
	}
}
