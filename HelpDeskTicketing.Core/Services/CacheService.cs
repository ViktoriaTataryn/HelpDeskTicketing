using HelpDeskTicketing.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HelpDeskTicketing.Core.Services;

public class CacheService:ICacheService
{
    public const string TICKET_KEY = "ticket";
    
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetCachedDataAsync<T>(string key, Func<Task<T>> onCacheMiss)
    {
        if (_cache.TryGetValue(key, out T value))
        {
            return value;
        }
        else
        {
            var data = await onCacheMiss();
            AddCache(key, data);
            return data;
        }
    }

    public void DeleteCachedData(string key)
    {
        _cache.Remove(key);
    }

    public void AddCache<T>(string key, T value)
    {
        var entry = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(240))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

        _cache.Set(key, value, entry);
    }
}