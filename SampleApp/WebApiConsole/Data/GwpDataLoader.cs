using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using WebApiConsole.Models;

namespace WebApiConsole.Data;

internal class GwpByCountryRecord
{
    public string Country { get; set; } = string.Empty;
    public string VariableId { get; set; } = string.Empty;
    public string VariableName { get; set; } = string.Empty;
    public string LineOfBusiness { get; set; } = string.Empty;
    public double? Y2000 { get; set; }
    public double? Y2001 { get; set; }
    public double? Y2002 { get; set; }
    public double? Y2003 { get; set; }
    public double? Y2004 { get; set; }
    public double? Y2005 { get; set; }
    public double? Y2006 { get; set; }
    public double? Y2007 { get; set; }
    public double? Y2008 { get; set; }
    public double? Y2009 { get; set; }
    public double? Y2010 { get; set; }
    public double? Y2011 { get; set; }
    public double? Y2012 { get; set; }
    public double? Y2013 { get; set; }
    public double? Y2014 { get; set; }
    public double? Y2015 { get; set; }
}

/// <summary>
/// ClassMap for case-insensitive header matching in CSV parsing
/// </summary>
internal class GwpByCountryMap : ClassMap<GwpByCountryRecord>
{
    public GwpByCountryMap()
    {
        Map(m => m.Country).Name("country");
        Map(m => m.VariableId).Name("variableId");
        Map(m => m.VariableName).Name("variableName");
        Map(m => m.LineOfBusiness).Name("lineOfBusiness");
        Map(m => m.Y2000).Name("Y2000");
        Map(m => m.Y2001).Name("Y2001");
        Map(m => m.Y2002).Name("Y2002");
        Map(m => m.Y2003).Name("Y2003");
        Map(m => m.Y2004).Name("Y2004");
        Map(m => m.Y2005).Name("Y2005");
        Map(m => m.Y2006).Name("Y2006");
        Map(m => m.Y2007).Name("Y2007");
        Map(m => m.Y2008).Name("Y2008");
        Map(m => m.Y2009).Name("Y2009");
        Map(m => m.Y2010).Name("Y2010");
        Map(m => m.Y2011).Name("Y2011");
        Map(m => m.Y2012).Name("Y2012");
        Map(m => m.Y2013).Name("Y2013");
        Map(m => m.Y2014).Name("Y2014");
        Map(m => m.Y2015).Name("Y2015");
    }
}

public class GwpDataLoader
{
    private readonly ILogger<GwpDataLoader> _logger;
    private List<GwpData>? _cachedData;
    private readonly string _dataFilePath;

    public GwpDataLoader(ILogger<GwpDataLoader> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Data", "gwpByCountry.csv");
    }

    public async Task<List<GwpData>> LoadDataAsync()
    {
        if (_cachedData != null)
        {
            _logger.LogDebug("Returning cached GWP data");
            return _cachedData;
        }

        _logger.LogInformation("Loading GWP data from {FilePath}", _dataFilePath);

        if (!File.Exists(_dataFilePath))
        {
            var errorMsg = $"GWP data file not found at {_dataFilePath}";
            _logger.LogError(errorMsg);
            throw new FileNotFoundException(errorMsg);
        }

        try
        {
            var records = new List<GwpData>();

            using (var reader = new StreamReader(_dataFilePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null, // Disable strict header validation
                };

                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<GwpByCountryMap>();
                    var csvRecords = csv.GetRecords<GwpByCountryRecord>().ToList();
                    AddRecordsFromCsv(csvRecords, records);
                }
            }

            _cachedData = records;
            _logger.LogInformation("Successfully loaded {RecordCount} GWP records", records.Count);
            return await Task.FromResult(records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading GWP data from CSV");
            throw;
        }
    }

    private static void AddRecordsFromCsv(IEnumerable<GwpByCountryRecord> csvRecords, List<GwpData> records)
    {
        foreach (var record in csvRecords)
        {
            var yearPremiums = new Dictionary<int, double?>
            {
                { 2008, record.Y2008 },
                { 2009, record.Y2009 },
                { 2010, record.Y2010 },
                { 2011, record.Y2011 },
                { 2012, record.Y2012 },
                { 2013, record.Y2013 },
                { 2014, record.Y2014 },
                { 2015, record.Y2015 }
            };

            foreach (var (year, premium) in yearPremiums)
            {
                if (!premium.HasValue || premium.Value <= 0)
                    continue;

                records.Add(new GwpData
                {
                    Country = record.Country,
                    Lob = record.LineOfBusiness,
                    Year = year,
                    Premium = premium.Value
                });
            }
        }
    }

    public async Task<List<string>> GetAvailableCountriesAsync()
    {
        var data = await LoadDataAsync();
        return data.Select(d => d.Country).Distinct().OrderBy(c => c).ToList();
    }

    public async Task<List<string>> GetAvailableLobsAsync()
    {
        var data = await LoadDataAsync();
        return data.Select(d => d.Lob).Distinct().OrderBy(l => l).ToList();
    }
}
