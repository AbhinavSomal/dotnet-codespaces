using WebApiConsole.Models;

namespace WebApiConsole.Repositories;

public interface IGwpRepository
{
    Task<List<GwpData>> GetGwpDataAsync(string country, List<string> lobs);
}
