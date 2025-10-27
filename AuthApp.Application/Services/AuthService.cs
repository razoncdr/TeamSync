using AuthApp.Application.DTOs;
using AuthApp.Application.Interfaces.Services;
using AuthApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthApp.Application.Services
{
	internal class AuthService : IAuthService
	{
		public Task<User> RegisterAsync(RegisterUserDto dto)
		{
			const User user = new User({ id = 1, Name = "Rejwanul Haque", Email = "rejwanul7296@gmail.com", Password = "1234" })
			return user;
		}
	}
}
