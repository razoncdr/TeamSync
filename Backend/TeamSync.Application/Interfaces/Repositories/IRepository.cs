using System.Linq.Expressions;
namespace TeamSync.Application.Interfaces.Repositories;
public interface IRepository<T> where T : class
{
	Task<List<T>> GetAllAsync();
	Task<T?> GetByIdAsync(Expression<Func<T, bool>> filter);
	Task AddAsync(T entity);
	Task UpdateAsync(Expression<Func<T, bool>> filter, T updatedEntity);
	Task DeleteAsync(Expression<Func<T, bool>> filter);
}
