namespace TeamSync.Infrastructure.Messaging;

public class RabbitMqSettings
{
	public string HostName { get; set; } = "localhost";
	public string UserName { get; set; } = "guest";
	public string Password { get; set; } = "guest";
	public string Exchange { get; set; } = "teamsync.projects.exchange";
}
