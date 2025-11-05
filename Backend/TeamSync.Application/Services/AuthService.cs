using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Services
{
	public class AuthService : IAuthService
	{
		private IUserRepository userRepository;
		public AuthService(IUserRepository _userRepository) {
			userRepository = _userRepository;
		}
		public async Task<User> RegisterAsync(RegisterUserDto dto)
		{
			// check if the user exists in db or not

			// if not, create a new user 
			using var hmac = new System.Security.Cryptography.HMACSHA512();
			var user = new User{ Name = dto.Name,
				Email = dto.Email,
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
				PasswordSalt = hmac.Key
			};

			await userRepository.AddAsync(user);

			return user;
		}
	}
}
