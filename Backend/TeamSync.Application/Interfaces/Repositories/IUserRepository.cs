using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User?> GetByEmailAsync(string email);
	}
}
