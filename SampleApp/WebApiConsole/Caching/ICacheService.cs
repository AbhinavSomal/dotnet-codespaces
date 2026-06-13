namespace WebApiConsole.Caching;

/// <summary>
/// Cache service interface for caching API responses
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from the cache
    /// </summary>
    TValue? Get<TValue>(string key);

    /// <summary>
    /// Sets a value in the cache
    /// </summary>
    void Set<TValue>(string key, TValue value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a value from the cache
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Gets a value from cache or executes the factory function
    /// </summary>
    Task<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory, TimeSpan? expiration = null);
}
