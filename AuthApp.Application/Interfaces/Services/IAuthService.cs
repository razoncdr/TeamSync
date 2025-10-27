using AuthApp.Application.DTOs;
using AuthApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthApp.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterUserDto dto);
    }
}
