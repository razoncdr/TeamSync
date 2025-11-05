using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TeamSync.Infrastructure.Settings;

namespace TeamSync.Infrastructure.Persistence
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		public MongoDbContext(IOptions<MongoDBSettings> options)
		{
			var settings = options.Value;
			var client = new MongoClient(settings.ConnectionString);
			_database = client.GetDatabase(settings.DatabaseName);
		}

		public IMongoCollection<T> GetCollection<T>(string name)
			=> _database.GetCollection<T>(name);
	}
}
