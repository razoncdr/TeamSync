using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IJwtService
	{
		string GenerateToken(User user);
	}
}
