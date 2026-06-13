using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApiConsole.Caching;
using WebApiConsole.Controllers;
using WebApiConsole.Models;
using WebApiConsole.Services;
using Xunit;

namespace WebApiConsole.Tests.Controllers;

public class CountryGwpControllerTests
{
    private readonly Mock<IGwpService> _mockService;
    private readonly Mock<ILogger<CountryGwpController>> _mockLogger;
    private readonly CountryGwpController _controller;

    public CountryGwpControllerTests()
    {
        _mockService = new Mock<IGwpService>();
        _mockLogger = new Mock<ILogger<CountryGwpController>>();
        _controller = new CountryGwpController(_mockService.Object, _mockLogger.Object);

        // Setup default HttpContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task GetAverageGwp_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "ae",
            Lob = new List<string> { "transport", "property" }
        };

        var expectedResponse = new GwpAverageResponse
        {
            { "transport", 446001906.1 },
            { "property", 1175000 }
        };

        _mockService.Setup(s => s.GetAverageGwpAsync(It.IsAny<GwpAverageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAverageGwp(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        var returnedResponse = Assert.IsType<GwpAverageResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnedResponse);
    }

    [Fact]
    public async Task GetAverageGwp_WithInvalidCountry_ReturnsBadRequest()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "",
            Lob = new List<string> { "transport" }
        };

        _mockService
            .Setup(s => s.GetAverageGwpAsync(It.IsAny<GwpAverageRequest>()))
            .ThrowsAsync(new ArgumentException("Country code cannot be empty or null"));

        // Act
        var result = await _controller.GetAverageGwp(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Contains("Invalid request data", errorResponse.Message);
    }

    [Fact]
    public async Task GetAverageGwp_WithEmptyLob_ReturnsBadRequest()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "ae",
            Lob = new List<string>()
        };

        _mockService
            .Setup(s => s.GetAverageGwpAsync(It.IsAny<GwpAverageRequest>()))
            .ThrowsAsync(new ArgumentException("At least one line of business must be specified"));

        // Act
        var result = await _controller.GetAverageGwp(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task GetAverageGwp_WithUnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "ae",
            Lob = new List<string> { "transport" }
        };

        _mockService
            .Setup(s => s.GetAverageGwpAsync(It.IsAny<GwpAverageRequest>()))
            .ThrowsAsync(new InvalidOperationException("Unexpected database error"));

        // Act
        var result = await _controller.GetAverageGwp(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Contains("An error occurred", errorResponse.Message);
    }

    [Fact]
    public async Task GetAverageGwp_WithMultipleLobs_ReturnsAllAverages()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "ae",
            Lob = new List<string> { "transport", "property", "liability" }
        };

        var expectedResponse = new GwpAverageResponse
        {
            { "transport", 446001906.1 },
            { "property", 1175000 },
            { "liability", 634545022.9 }
        };

        _mockService.Setup(s => s.GetAverageGwpAsync(It.IsAny<GwpAverageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAverageGwp(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResponse = Assert.IsType<GwpAverageResponse>(okResult.Value);
        Assert.Equal(3, returnedResponse.Count);
    }
}
