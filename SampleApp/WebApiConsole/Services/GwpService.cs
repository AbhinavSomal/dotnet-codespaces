using WebApiConsole.Caching;
using WebApiConsole.Models;
using WebApiConsole.Repositories;

namespace WebApiConsole.Services;

public interface IGwpService
{
    Task<GwpAverageResponse> GetAverageGwpAsync(GwpAverageRequest request);
}

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

    public async Task<GwpAverageResponse> GetAverageGwpAsync(GwpAverageRequest request)
    {
        ValidateRequest(request);

        var cacheKey = GenerateCacheKey(request);

        var cachedResult = _cacheService.Get<GwpAverageResponse>(cacheKey);
        if (cachedResult != null)
        {
            _logger.LogInformation("Returning cached GWP average for country: {Country}, LOBs: {Lobs}",
                request.Country, string.Join(",", request.Lob));
            return cachedResult;
        }

        _logger.LogInformation("Calculating GWP average for country: {Country}, LOBs: {Lobs}",
            request.Country, string.Join(",", request.Lob));

        var gwpData = await _repository.GetGwpDataAsync(request.Country, request.Lob);

        if (gwpData.Count == 0)
        {
            _logger.LogWarning("No GWP data found for country: {Country}, LOBs: {Lobs}",
                request.Country, string.Join(",", request.Lob));
            return new GwpAverageResponse();
        }

        var response = new GwpAverageResponse();
        var groupedByLob = gwpData.GroupBy(d => d.Lob, StringComparer.OrdinalIgnoreCase);

        foreach (var lobGroup in groupedByLob)
        {
            var average = lobGroup.Average(d => d.Premium);
            response[lobGroup.Key] = Math.Round(average, 1);
            _logger.LogDebug("Calculated average for LOB {Lob}: {Average}", lobGroup.Key, average);
        }

        _cacheService.Set(cacheKey, response, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return response;
    }

    private void ValidateRequest(GwpAverageRequest request)
    {
        // TODO: Add more validation if needed (e.g., check for valid country codes, valid LOBs, etc.)
        if (string.IsNullOrWhiteSpace(request.Country))
        {
            throw new ArgumentException("Country code cannot be empty or null", nameof(request.Country));
        }
    }

    private string GenerateCacheKey(GwpAverageRequest request)
    {
        var sortedLobs = string.Join("_", request.Lob.OrderBy(l => l, StringComparer.OrdinalIgnoreCase));
        return $"{request.Country}_{sortedLobs}".ToLower();
    }
}
