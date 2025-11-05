using TeamSync.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterUserDto dto);
    }
}
