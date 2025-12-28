using Pipelines.Sockets.Unofficial.Arenas;
using TeamSync.Application.Common.Exceptions;
using TeamSync.Application.DTOs.User;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Application.Services
{
	public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IEventPublisher _publisher;

		public UserService(
			IUserRepository userRepository,
            IRedisCacheService redisCacheService, 
			IEventPublisher publisher
			)
		{
			_userRepository = userRepository;
            _redisCacheService = redisCacheService;
			_publisher = publisher;
		}

        public async Task<List<UserBasicInfoDto>> GetByIdsAsync(List<string> userIds)
        {
            var users = await _userRepository.GetByIdsAsync(userIds);

            if (users == null || !users.Any())
            {
                throw new NotFoundException("Users not found");
            }

            var dto = users.Select(u => new UserBasicInfoDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name
            }).ToList();

            return dto;
        }

    }
}
