using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<List<User>> GetAllAsync();
		Task<User?> GetByEmailAsync(string email);
		Task AddAsync(User user);
		Task DeleteAsync(string email);
	}
}
