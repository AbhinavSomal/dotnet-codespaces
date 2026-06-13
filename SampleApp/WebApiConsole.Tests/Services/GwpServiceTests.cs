using Microsoft.Extensions.Logging;
using Moq;
using WebApiConsole.Caching;
using WebApiConsole.Models;
using WebApiConsole.Repositories;
using WebApiConsole.Services;
using Xunit;

namespace WebApiConsole.Tests.Services;

public class GwpServiceTests
{
    private readonly Mock<IGwpRepository> _mockRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ILogger<GwpService>> _mockLogger;
    private readonly GwpService _service;

    public GwpServiceTests()
    {
        _mockRepository = new Mock<IGwpRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _mockLogger = new Mock<ILogger<GwpService>>();
        _service = new GwpService(_mockRepository.Object, _mockCacheService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAverageGwp_WithValidRequest_ReturnsAverageValues()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "ae",
            Lob = new List<string> { "transport", "property" }
        };

        var gwpData = new List<GwpData>
        {
            new() { Country = "ae", Lob = "transport", Year = 2008, Premium = 400000000 },
            new() { Country = "ae", Lob = "transport", Year = 2009, Premium = 420000000 },
            new() { Country = "ae", Lob = "transport", Year = 2010, Premium = 440000000 },
            new() { Country = "ae", Lob = "transport", Year = 2011, Premium = 450000000 },
            new() { Country = "ae", Lob = "transport", Year = 2012, Premium = 460000000 },
            new() { Country = "ae", Lob = "transport", Year = 2013, Premium = 470000000 },
            new() { Country = "ae", Lob = "transport", Year = 2014, Premium = 480000000 },
            new() { Country = "ae", Lob = "transport", Year = 2015, Premium = 490000000 },
            new() { Country = "ae", Lob = "property", Year = 2008, Premium = 1000000 },
            new() { Country = "ae", Lob = "property", Year = 2009, Premium = 1050000 },
            new() { Country = "ae", Lob = "property", Year = 2010, Premium = 1100000 },
            new() { Country = "ae", Lob = "property", Year = 2011, Premium = 1150000 },
            new() { Country = "ae", Lob = "property", Year = 2012, Premium = 1200000 },
            new() { Country = "ae", Lob = "property", Year = 2013, Premium = 1250000 },
            new() { Country = "ae", Lob = "property", Year = 2014, Premium = 1300000 },
            new() { Country = "ae", Lob = "property", Year = 2015, Premium = 1350000 }
        };

        _mockCacheService.Setup(c => c.Get<GwpAverageResponse>(It.IsAny<string>())).Returns((GwpAverageResponse?)null);
        _mockRepository.Setup(r => r.GetGwpDataAsync("ae", It.IsAny<List<string>>())).ReturnsAsync(gwpData);

        // Act
        var result = await _service.GetAverageGwpAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("transport", result.Keys);
        Assert.Contains("property", result.Keys);
        // Transport average: (400+420+440+450+460+470+480+490)M / 8 = 451.25M
        Assert.Equal(451250000, result["transport"]);
        // Property average: (1+1.05+1.1+1.15+1.2+1.25+1.3+1.35)M / 8 = 1.15M
        Assert.Equal(1175000, result["property"]);
        _mockRepository.Verify(r => r.GetGwpDataAsync("ae", It.IsAny<List<string>>()), Times.Once);
        _mockCacheService.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<GwpAverageResponse>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetAverageGwp_WithCachedResult_ReturnsCachedValue()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "ae",
            Lob = new List<string> { "transport" }
        };

        var cachedResponse = new GwpAverageResponse { { "transport", 446001906.1 } };
        _mockCacheService.Setup(c => c.Get<GwpAverageResponse>(It.IsAny<string>())).Returns(cachedResponse);

        // Act
        var result = await _service.GetAverageGwpAsync(request);

        // Assert
        Assert.Equal(cachedResponse, result);
        _mockRepository.Verify(r => r.GetGwpDataAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task GetAverageGwp_WithEmptyCountry_ThrowsArgumentException()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = string.Empty,
            Lob = new List<string> { "transport" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetAverageGwpAsync(request));
    }

    [Fact]
    public async Task GetAverageGwp_WithEmptyLob_ThrowsArgumentException()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "ae",
            Lob = new List<string>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetAverageGwpAsync(request));
    }

    [Fact]
    public async Task GetAverageGwp_WithNoDataFound_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "invalid",
            Lob = new List<string> { "transport" }
        };

        _mockCacheService.Setup(c => c.Get<GwpAverageResponse>(It.IsAny<string>())).Returns((GwpAverageResponse?)null);
        _mockRepository.Setup(r => r.GetGwpDataAsync("invalid", It.IsAny<List<string>>())).ReturnsAsync(new List<GwpData>());

        // Act
        var result = await _service.GetAverageGwpAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAverageGwp_CaseInsensitiveCountryAndLob_Works()
    {
        // Arrange
        var request = new GwpAverageRequest
        {
            Country = "AE",
            Lob = new List<string> { "TRANSPORT" }
        };

        var gwpData = new List<GwpData>
        {
            new() { Country = "ae", Lob = "transport", Year = 2008, Premium = 400000000 },
            new() { Country = "ae", Lob = "transport", Year = 2015, Premium = 490000000 }
        };

        _mockCacheService.Setup(c => c.Get<GwpAverageResponse>(It.IsAny<string>())).Returns((GwpAverageResponse?)null);
        _mockRepository.Setup(r => r.GetGwpDataAsync(It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync(gwpData);

        // Act
        var result = await _service.GetAverageGwpAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}
