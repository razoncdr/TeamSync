using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterUserDto dto);
		Task<string?> LoginAsync(LoginUserDto dto);
	}
}
