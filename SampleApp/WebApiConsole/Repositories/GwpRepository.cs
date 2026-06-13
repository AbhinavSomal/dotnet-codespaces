using WebApiConsole.Data;
using WebApiConsole.Models;

namespace WebApiConsole.Repositories;

/// <summary>
/// Repository implementation for GWP data access
/// Decouples data access logic from business logic
/// </summary>
public class GwpRepository : IGwpRepository
{
    private readonly GwpDataLoader _dataLoader;
    private readonly ILogger<GwpRepository> _logger;

    public GwpRepository(GwpDataLoader dataLoader, ILogger<GwpRepository> logger)
    {
        _dataLoader = dataLoader;
        _logger = logger;
    }

    /// <summary>
    /// Gets GWP data for a specific country and lines of business
    /// </summary>
    public async Task<List<GwpData>> GetGwpDataAsync(string country, List<string> lobs)
    {
        _logger.LogDebug("Fetching GWP data for country: {Country}, LOBs: {Lobs}", country, string.Join(",", lobs));

        var allData = await _dataLoader.LoadDataAsync();

        var filteredData = allData
            .Where(d => d.Country.Equals(country, StringComparison.OrdinalIgnoreCase) &&
                       lobs.Contains(d.Lob, StringComparer.OrdinalIgnoreCase))
            .ToList();

        _logger.LogDebug("Found {RecordCount} records matching criteria", filteredData.Count);
        return filteredData;
    }

    /// <summary>
    /// Gets all available countries
    /// </summary>
    public async Task<List<string>> GetAvailableCountriesAsync()
    {
        return await _dataLoader.GetAvailableCountriesAsync();
    }

    /// <summary>
    /// Gets all available lines of business
    /// </summary>
    public async Task<List<string>> GetAvailableLobsAsync()
    {
        return await _dataLoader.GetAvailableLobsAsync();
    }
}
