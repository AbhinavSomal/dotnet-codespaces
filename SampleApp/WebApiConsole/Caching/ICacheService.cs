namespace WebApiConsole.Caching;

public interface ICacheService
{
    TValue? Get<TValue>(string key);

    void Set<TValue>(string key, TValue value, TimeSpan? expiration = null);

    void Remove(string key);

    Task<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory, TimeSpan? expiration = null);
}
