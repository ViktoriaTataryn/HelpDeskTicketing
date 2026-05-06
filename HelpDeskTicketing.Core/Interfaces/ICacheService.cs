namespace HelpDeskTicketing.Core.Interfaces;

public interface ICacheService
{
    Task<T> GetCachedDataAsync<T>(string key, Func<Task<T>> onCacheMiss);
    
    void DeleteCachedData(string key);

    void AddCache<T>(string key, T value);
}