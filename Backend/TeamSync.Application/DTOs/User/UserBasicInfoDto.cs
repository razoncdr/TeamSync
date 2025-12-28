using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSync.Application.DTOs.User
{
    public class UserBasicInfoDto
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
