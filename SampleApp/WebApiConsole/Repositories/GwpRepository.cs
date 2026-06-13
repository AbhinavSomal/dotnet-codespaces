using WebApiConsole.Data;
using WebApiConsole.Models;

namespace WebApiConsole.Repositories;

public class GwpRepository : IGwpRepository
{
    private readonly GwpDataLoader _dataLoader;

    public GwpRepository(GwpDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
    }

    public async Task<List<GwpData>> GetGwpDataAsync(string country, List<string> lobs)
    {
        var allData = await _dataLoader.LoadDataAsync();

        var filteredData = allData
            .Where(d => d.Country.Equals(country, StringComparison.OrdinalIgnoreCase) &&
                       lobs.Contains(d.Lob, StringComparer.OrdinalIgnoreCase))
            .ToList();
        return filteredData;
    }
}
