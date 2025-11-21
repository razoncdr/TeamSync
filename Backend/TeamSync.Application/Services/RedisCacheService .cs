using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TeamSync.Application.Interfaces.Services;

namespace TeamSync.Application.Services
{
	public class RedisCacheService : IRedisCacheService
	{
		private readonly IConnectionMultiplexer _redis;

		public RedisCacheService(IConnectionMultiplexer redis)
		{
			_redis = redis;
		}

		public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
		{
			var db = _redis.GetDatabase();
			var json = JsonSerializer.Serialize(value);

			await db.StringSetAsync(
				key: key,
				value: json,
				expiry: expiry,
				when: When.Always,
				flags: CommandFlags.None
			);
		}

		public async Task<T?> GetAsync<T>(string key)
		{
			var db = _redis.GetDatabase();
			var json = await db.StringGetAsync(key);
			if (json.IsNullOrEmpty) return default;
			return JsonSerializer.Deserialize<T>(json!);
		}

		public async Task RemoveAsync(string key)
		{
			var db = _redis.GetDatabase();
			await db.KeyDeleteAsync(key);
		}
	}
}
