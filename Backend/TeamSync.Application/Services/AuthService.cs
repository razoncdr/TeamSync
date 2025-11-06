using System.Security.Cryptography;
using System.Text;
using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly IJwtService _jwtService;

		public AuthService(IUserRepository userRepository, IJwtService jwtService)
		{
			_userRepository = userRepository;
			_jwtService = jwtService;
		}

		public async Task<User> RegisterAsync(RegisterUserDto dto)
		{
			using var hmac = new HMACSHA512();

			var user = new User
			{
				Name = dto.Name,
				Email = dto.Email,
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
				PasswordSalt = hmac.Key
			};

			await _userRepository.AddAsync(user);
			return user;
		}

		public async Task<string?> LoginAsync(LoginUserDto dto)
		{
			var user = await _userRepository.GetByEmailAsync(dto.Email);
			if (user == null) return null;

			using var hmac = new HMACSHA512(user.PasswordSalt);
			var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

			if (!computedHash.SequenceEqual(user.PasswordHash))
				return null;

			return _jwtService.GenerateToken(user);
		}
	}
}
