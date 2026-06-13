namespace WebApiConsole.Models;

/// <summary>
/// Response model for GWP average calculation
/// </summary>
public class GwpAverageResponse : Dictionary<string, double>
{
}

/// <summary>
/// Entity representing Gross Written Premium data
/// </summary>
public class GwpData
{
    public required string Country { get; set; }
    public required string Lob { get; set; }
    public int Year { get; set; }
    public double Premium { get; set; }
}

/// <summary>
/// Error response model
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public List<string>? Details { get; set; }
    public string? TraceId { get; set; }
}
