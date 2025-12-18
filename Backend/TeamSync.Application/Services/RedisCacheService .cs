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

        public async Task<Dictionary<string, T>> GetManyAsync<T>(
    IEnumerable<string> keys)
        {
            var db = _redis.GetDatabase();

            var redisKeys = keys
                .Select(k => (RedisKey)k)
                .ToArray();

            if (redisKeys.Length == 0)
                return new Dictionary<string, T>();

            RedisValue[] values = await db.StringGetAsync(redisKeys);

            var result = new Dictionary<string, T>();

            for (int i = 0; i < redisKeys.Length; i++)
            {
                if (!values[i].IsNullOrEmpty)
                {
                    var value = JsonSerializer.Deserialize<T>(values[i]!);
                    if (value != null)
                    {
                        result[redisKeys[i]!] = value;
                    }
                }
            }

            return result;
        }



        public async Task RemoveAsync(string key)
		{
			var db = _redis.GetDatabase();
			await db.KeyDeleteAsync(key);
		}

        public async Task ListRightPushAsync<T>(string key, T value)
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.ListRightPushAsync(key, json);
        }

        public async Task ListTrimAsync(string key, long start, long stop)
        {
            var db = _redis.GetDatabase();
            await db.ListTrimAsync(key, start, stop);
        }
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var values = await db.ListRangeAsync(key, 0, -1);
            return values
                .Where(v => !v.IsNullOrEmpty)
                .Select(v => JsonSerializer.Deserialize<T>(v)!)
                .ToList();
        }
    }
}
