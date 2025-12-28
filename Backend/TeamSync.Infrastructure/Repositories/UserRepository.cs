using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;
using MongoDB.Driver;

namespace TeamSync.Infrastructure.Repositories
{
	public class UserRepository : MongoRepository<User>, IUserRepository
	{
		public UserRepository(IMongoDatabase database)
			: base(database, "Users") { }

		public Task<User?> GetByEmailAsync(string email) =>
			Collection.Find(u => u.Email == email).FirstOrDefaultAsync();

		public Task<List<User>> GetByIdsAsync(List<string> ids) =>
			Collection.Find(u => ids.Contains(u.Id)).ToListAsync();
    }
}
