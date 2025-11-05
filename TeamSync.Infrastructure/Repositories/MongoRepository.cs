using MongoDB.Driver;
using System.Linq.Expressions;

namespace TeamSync.Infrastructure.Repositories
{
	public class MongoRepository<T> where T : class
	{
		private readonly IMongoCollection<T> _collection;

		public MongoRepository(IMongoDatabase database, string collectionName)
		{
			_collection = database.GetCollection<T>(collectionName);
		}

		public async Task<List<T>> GetAllAsync() =>
			await _collection.Find(_ => true).ToListAsync();

		public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> filter) =>
			await _collection.Find(filter).FirstOrDefaultAsync();

		public async Task AddAsync(T entity) =>
			await _collection.InsertOneAsync(entity);

		public async Task UpdateAsync(Expression<Func<T, bool>> filter, T updatedEntity) =>
			await _collection.ReplaceOneAsync(filter, updatedEntity);

		public async Task DeleteAsync(Expression<Func<T, bool>> filter) =>
			await _collection.DeleteOneAsync(filter);
	}
}
