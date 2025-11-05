using TeamSync.Application.Interfaces.Repositories;
using MongoDB.Driver;
using TeamSync.Domain.Entities;

namespace TeamSync.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly IMongoCollection<User> _users;

		public UserRepository(IMongoDatabase database)
		{
			_users = database.GetCollection<User>("Users");
		}

		public async Task<List<User>> GetAllAsync() =>
			await _users.Find(_ => true).ToListAsync();

		public async Task<User?> GetByEmailAsync(string email) =>
			await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

		public async Task AddAsync(User user) =>
			await _users.InsertOneAsync(user);

		public async Task DeleteAsync(string email) =>
			await _users.DeleteOneAsync(u => u.Email == email);
	}
}
