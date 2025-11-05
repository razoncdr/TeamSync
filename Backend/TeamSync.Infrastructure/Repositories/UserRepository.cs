using TeamSync.Application.Interfaces.Repositories;
using MongoDB.Driver;
using TeamSync.Domain.Entities;

namespace TeamSync.Infrastructure.Repositories
{
	public class UserRepository : MongoRepository<User>, IUserRepository
	{
		private readonly IMongoCollection<User> _users;

		public UserRepository(IMongoDatabase database)
			: base(database, "Users")
		{
			_users = database.GetCollection<User>("Users");
		}

		public async Task<User?> GetByEmailAsync(string email) =>
			await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
	}
}
