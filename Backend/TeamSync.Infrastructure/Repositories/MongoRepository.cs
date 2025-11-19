using MongoDB.Driver;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;

namespace TeamSync.Infrastructure.Repositories
{
	public class MongoRepository<T> : IRepository<T> where T : BaseEntity
	{
		protected readonly IMongoCollection<T> Collection;

		public MongoRepository(IMongoDatabase database, string collectionName)
		{
			Collection = database.GetCollection<T>(collectionName);
		}
		public Task<bool> ExistsAsync(string id) =>
			Collection.Find(e => e.Id == id).AnyAsync();

		public Task<List<T>> GetAllAsync() =>
			Collection.Find(_ => true).ToListAsync();

		public Task<T?> GetByIdAsync(string id) =>
			Collection.Find(e => e.Id == id).FirstOrDefaultAsync();

		public Task AddAsync(T entity) =>
			Collection.InsertOneAsync(entity);

		public Task UpdateAsync(T entity)
		{
			entity.UpdatedAt = DateTime.UtcNow; // automatically set UpdatedAt
			return Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
		}

		public Task DeleteAsync(string id) =>
			Collection.DeleteOneAsync(e => e.Id == id);
	}
}
