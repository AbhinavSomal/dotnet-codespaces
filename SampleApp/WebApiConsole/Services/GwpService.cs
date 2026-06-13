using WebApiConsole.Caching;
using WebApiConsole.Models;
using WebApiConsole.Repositories;

namespace WebApiConsole.Services;

/// <summary>
/// Service interface for GWP business logic
/// </summary>
public interface IGwpService
{
    /// <summary>
    /// Calculates average GWP for specified country and lines of business
    /// </summary>
    Task<GwpAverageResponse> GetAverageGwpAsync(GwpAverageRequest request);
}

/// <summary>
/// Service implementation for GWP business logic
/// </summary>
public class GwpService : IGwpService
{
    private readonly IGwpRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GwpService> _logger;
    private const int CacheExpirationMinutes = 60;

    public GwpService(IGwpRepository repository, ICacheService cacheService, ILogger<GwpService> logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Calculates average GWP for specified country and lines of business
    /// Results are cached for subsequent requests
    /// </summary>
    public async Task<GwpAverageResponse> GetAverageGwpAsync(GwpAverageRequest request)
    {
        ValidateRequest(request);

        var cacheKey = GenerateCacheKey(request);

        // Try to get from cache
        var cachedResult = _cacheService.Get<GwpAverageResponse>(cacheKey);
        if (cachedResult != null)
        {
            _logger.LogInformation("Returning cached GWP average for country: {Country}, LOBs: {Lobs}",
                request.Country, string.Join(",", request.Lob));
            return cachedResult;
        }

        _logger.LogInformation("Calculating GWP average for country: {Country}, LOBs: {Lobs}",
            request.Country, string.Join(",", request.Lob));

        // Fetch data from repository
        var gwpData = await _repository.GetGwpDataAsync(request.Country, request.Lob);

        if (gwpData.Count == 0)
        {
            _logger.LogWarning("No GWP data found for country: {Country}, LOBs: {Lobs}",
                request.Country, string.Join(",", request.Lob));
            return new GwpAverageResponse();
        }

        // Calculate averages
        var response = new GwpAverageResponse();
        var groupedByLob = gwpData.GroupBy(d => d.Lob, StringComparer.OrdinalIgnoreCase);

        foreach (var lobGroup in groupedByLob)
        {
            var average = lobGroup.Average(d => d.Premium);
            response[lobGroup.Key] = Math.Round(average, 1);
            _logger.LogDebug("Calculated average for LOB {Lob}: {Average}", lobGroup.Key, average);
        }

        // Cache the result
        _cacheService.Set(cacheKey, response, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return response;
    }

    /// <summary>
    /// Validates the request data
    /// </summary>
    private void ValidateRequest(GwpAverageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Country))
        {
            throw new ArgumentException("Country code cannot be empty or null", nameof(request.Country));
        }

        if (request.Lob == null || request.Lob.Count == 0)
        {
            throw new ArgumentException("At least one line of business must be specified", nameof(request.Lob));
        }

        if (request.Lob.Any(l => string.IsNullOrWhiteSpace(l)))
        {
            throw new ArgumentException("Lines of business cannot contain empty or null values", nameof(request.Lob));
        }
    }

    /// <summary>
    /// Generates a cache key for the request
    /// </summary>
    private string GenerateCacheKey(GwpAverageRequest request)
    {
        var sortedLobs = string.Join("_", request.Lob.OrderBy(l => l, StringComparer.OrdinalIgnoreCase));
        return $"{request.Country}_{sortedLobs}".ToLower();
    }
}
