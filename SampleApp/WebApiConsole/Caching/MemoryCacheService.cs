using Microsoft.Extensions.Caching.Memory;

namespace WebApiConsole.Caching;

/// <summary>
/// In-memory cache service implementation
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    private const string CacheKeyPrefix = "gwp_api_";

    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    /// <summary>
    /// Gets a value from the cache
    /// </summary>
    public TValue? Get<TValue>(string key)
    {
        var cacheKey = GetCacheKey(key);
        if (_memoryCache.TryGetValue(cacheKey, out TValue? value))
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return value;
        }

        _logger.LogDebug("Cache miss for key: {Key}", key);
        return default;
    }

    /// <summary>
    /// Sets a value in the cache
    /// </summary>
    public void Set<TValue>(string key, TValue value, TimeSpan? expiration = null)
    {
        var cacheKey = GetCacheKey(key);
        var cacheOptions = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            // Default 1 hour expiration
            cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
        }

        _memoryCache.Set(cacheKey, value, cacheOptions);
        _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expiration);
    }

    /// <summary>
    /// Removes a value from the cache
    /// </summary>
    public void Remove(string key)
    {
        var cacheKey = GetCacheKey(key);
        _memoryCache.Remove(cacheKey);
        _logger.LogDebug("Removed cache entry for key: {Key}", key);
    }

    /// <summary>
    /// Gets a value from cache or executes the factory function
    /// </summary>
    public async Task<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory, TimeSpan? expiration = null)
    {
        var cachedValue = Get<TValue>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();
        Set(key, value, expiration);
        return value;
    }

    private string GetCacheKey(string key) => $"{CacheKeyPrefix}{key}";
}
