namespace WebApiConsole.Models;

public class GwpAverageRequest
{
    public required string Country { get; set; }

    public required List<string> Lob { get; set; }
}
