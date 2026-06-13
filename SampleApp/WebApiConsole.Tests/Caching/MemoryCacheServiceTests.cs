using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using WebApiConsole.Caching;
using Xunit;

namespace WebApiConsole.Tests.Caching;

public class MemoryCacheServiceTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<MemoryCacheService>> _mockLogger;
    private readonly MemoryCacheService _cacheService;

    public MemoryCacheServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockLogger = new Mock<ILogger<MemoryCacheService>>();
        _cacheService = new MemoryCacheService(_memoryCache, _mockLogger.Object);
    }

    [Fact]
    public void Set_And_Get_StoresAndRetrievesValue()
    {
        // Arrange
        var key = "test_key";
        var value = "test_value";

        // Act
        _cacheService.Set(key, value);
        var result = _cacheService.Get<string>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Get_NonExistentKey_ReturnsNull()
    {
        // Act
        var result = _cacheService.Get<string>("non_existent_key");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Remove_DeletesValue()
    {
        // Arrange
        var key = "test_key";
        _cacheService.Set(key, "value");

        // Act
        _cacheService.Remove(key);
        var result = _cacheService.Get<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrSetAsync_WithCachedValue_ReturnsCached()
    {
        // Arrange
        var key = "test_key";
        var cachedValue = "cached_value";
        _cacheService.Set(key, cachedValue);
        var factoryCalled = false;

        // Act
        var result = await _cacheService.GetOrSetAsync(key, async () =>
        {
            factoryCalled = true;
            return await Task.FromResult("factory_value");
        });

        // Assert
        Assert.Equal(cachedValue, result);
        Assert.False(factoryCalled);
    }

    [Fact]
    public async Task GetOrSetAsync_WithoutCachedValue_CallsFactory()
    {
        // Arrange
        var key = "test_key";
        var expectedValue = "factory_value";

        // Act
        var result = await _cacheService.GetOrSetAsync(key, async () =>
        {
            return await Task.FromResult(expectedValue);
        });

        // Assert
        Assert.Equal(expectedValue, result);
        var cachedValue = _cacheService.Get<string>(key);
        Assert.Equal(expectedValue, cachedValue);
    }

    [Fact]
    public void Set_WithExpiration_ExpiresValue()
    {
        // Arrange
        var key = "expiring_key";
        var value = "expiring_value";
        var expiration = TimeSpan.FromMilliseconds(100);

        // Act
        _cacheService.Set(key, value, expiration);
        var resultBefore = _cacheService.Get<string>(key);

        System.Threading.Thread.Sleep(150); // Wait for expiration
        var resultAfter = _cacheService.Get<string>(key);

        // Assert
        Assert.NotNull(resultBefore);
        Assert.Null(resultAfter);
    }
}
