using System.Security.Cryptography;
using System.Text;
using TeamSync.Application.Common.Exceptions;
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
		private readonly IRedisCacheService _redisCacheService;

		public AuthService(IUserRepository userRepository, IJwtService jwtService, IRedisCacheService redisCacheService)
		{
			_userRepository = userRepository;
			_jwtService = jwtService;
			_redisCacheService = redisCacheService;
		}

		public async Task<User?> GetByEmailAsync(string email)
		{
			return await _userRepository.GetByEmailAsync(email);
		}

		public async Task<User> RegisterAsync(RegisterUserDto dto)
		{
			var existing = await _userRepository.GetByEmailAsync(dto.Email);
			if (existing != null)
				throw new ConflictException("Email already in use.");

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

		public async Task<string> LoginAsync(LoginUserDto dto)
		{
			// Try getting user from Redis first
			var cacheKey = $"user:{dto.Email}";
			var cachedUser = await _redisCacheService.GetAsync<User>(cacheKey);

			User user;
			if (cachedUser != null)
			{
				Console.WriteLine("--- cache hit ---");
				user = cachedUser;
			}
			else
			{
				Console.WriteLine("--- cache miss ---");
				user = await _userRepository.GetByEmailAsync(dto.Email);
				if (user == null)
					throw new UnauthorizedException("Invalid credentials.");

				using var hmac = new HMACSHA512(user.PasswordSalt);
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

				if (!computedHash.SequenceEqual(user.PasswordHash))
					throw new UnauthorizedException("Invalid credentials.");

				// Cache user info for 30 minutes
				await _redisCacheService.SetAsync(cacheKey, user, TimeSpan.FromMinutes(30));
			}

			return _jwtService.GenerateToken(user);
		}

		public async Task RemoveUserCacheAsync(string email)
		{
			await _redisCacheService.RemoveAsync($"user:{email}");
		}
	}
}
