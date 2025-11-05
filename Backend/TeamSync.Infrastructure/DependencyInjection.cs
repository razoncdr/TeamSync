using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Infrastructure.Repositories;
using TeamSync.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TeamSync.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
		{
			// Bind MongoDB settings
			services.Configure<MongoDBSettings>(config.GetSection("MongoDBSettings"));

			// MongoClient
			services.AddSingleton<IMongoClient>(sp =>
			{
				var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
				return new MongoClient(settings.ConnectionString);
			});

			// MongoDatabase
			services.AddScoped(sp =>
			{
				var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
				var client = sp.GetRequiredService<IMongoClient>();
				return client.GetDatabase(settings.DatabaseName);
			});

			// Register repositories
			services.AddScoped<IUserRepository, UserRepository>();

			return services;
		}
	}
}
