using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSync.Application.DTOs.User;

namespace TeamSync.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<List<UserBasicInfoDto>> GetByIdsAsync(List<string> userIds);
    }
}
