using WebApiConsole.Models;

namespace WebApiConsole.Repositories;

/// <summary>
/// Repository interface for GWP data access
/// </summary>
public interface IGwpRepository
{
    /// <summary>
    /// Gets GWP data for a specific country and lines of business
    /// </summary>
    Task<List<GwpData>> GetGwpDataAsync(string country, List<string> lobs);

    /// <summary>
    /// Gets all available countries
    /// </summary>
    Task<List<string>> GetAvailableCountriesAsync();

    /// <summary>
    /// Gets all available lines of business
    /// </summary>
    Task<List<string>> GetAvailableLobsAsync();
}
