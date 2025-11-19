using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IRepository<T> where T : BaseEntity
	{
		Task<bool> ExistsAsync(string id);
		Task<List<T>> GetAllAsync();
		Task<T?> GetByIdAsync(string id);
		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(string id);
	}
}
