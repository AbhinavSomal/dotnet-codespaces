using Microsoft.AspNetCore.Mvc;
using WebApiConsole.Models;
using WebApiConsole.Services;

namespace WebApiConsole.Controllers;


[ApiController]
[Route("server/api/gwp")]
public class CountryGwpController : ControllerBase
{
    private readonly IGwpService _gwpService;
    private readonly ILogger<CountryGwpController> _logger;

    public CountryGwpController(IGwpService gwpService, ILogger<CountryGwpController> logger)
    {
        _gwpService = gwpService;
        _logger = logger;
    }

    /// <summary>
    /// Calculates average GWP for specified country and lines of business (2008-2015)
    /// </summary>
    /// <param name="request">Request containing country code and lines of business</param>
    /// <returns>Dictionary of LOB to average GWP values</returns>
    /// <response code="200">Successfully calculated average GWP</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("avg")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<GwpAverageResponse>> GetAverageGwp([FromBody] GwpAverageRequest request)
    {
        try
        {
            _logger.LogInformation("Received GWP average request for country: {Country}, LOBs: {Lobs}",
                request.Country, string.Join(",", request.Lob ?? new List<string>()));

            var result = await _gwpService.GetAverageGwpAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request data");
            return BadRequest(new ErrorResponse
            {
                Message = "Invalid request data",
                Details = new List<string> { ex.Message },
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GWP average request");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Message = "An error occurred while processing your request",
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }
}
