namespace WebApiConsole.Models;

/// <summary>
/// Request model for calculating average GWP
/// </summary>
public class GwpAverageRequest
{
    /// <summary>
    /// Country code (e.g., "ae", "us", "uk")
    /// </summary>
    public required string Country { get; set; }

    /// <summary>
    /// List of lines of business (e.g., "property", "transport", "liability")
    /// </summary>
    public required List<string> Lob { get; set; }
}
