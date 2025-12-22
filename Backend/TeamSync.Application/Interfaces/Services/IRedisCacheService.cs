using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSync.Application.Interfaces.Services
{
	public interface IRedisCacheService
	{
		Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
		Task<T?> GetAsync<T>(string key);
		Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys);
        Task RemoveAsync(string key);
		Task ListRightPushAsync<T>(string key, T value);
        Task ListTrimAsync(string key, long start, long stop);
		Task<List<T>> ListRangeAsync<T>(string key);


    }

}
