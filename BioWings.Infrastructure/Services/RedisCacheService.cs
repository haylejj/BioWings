using BioWings.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace BioWings.Infrastructure.Services;

public class RedisCacheService(IDistributedCache persistentCache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await persistentCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        var options = new DistributedCacheEntryOptions();

        if (expirationTime.HasValue)
        {
            options.SetAbsoluteExpiration(expirationTime.Value);
        }
        else
        {
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(60)); // Varsayılan süre
        }

        await persistentCache.SetStringAsync(key, serializedValue, options);
    }

    public async Task RemoveAsync(string key)
    {
        await persistentCache.RemoveAsync(key);
    }
}
