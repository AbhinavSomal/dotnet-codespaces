namespace WebApiConsole.Models;

public class GwpAverageResponse : Dictionary<string, double>
{
}

public class GwpData
{
    public required string Country { get; set; }
    public required string Lob { get; set; }
    public int Year { get; set; }
    public double Premium { get; set; }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public List<string>? Details { get; set; }
    public string? TraceId { get; set; }
}
