using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User?> GetByEmailAsync(string email);
		Task<List<User>> GetByIdsAsync(List<string> ids);
	}
}
