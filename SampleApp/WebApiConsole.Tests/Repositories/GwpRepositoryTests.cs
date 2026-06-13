using Moq;
using WebApiConsole.Models;
using WebApiConsole.Repositories;
using Xunit;

namespace WebApiConsole.Tests.Repositories;

public class GwpRepositoryTests
{
    private readonly Mock<IGwpRepository> _mockRepository;

    public GwpRepositoryTests()
    {
        _mockRepository = new Mock<IGwpRepository>();
    }

    [Fact]
    public async Task GetGwpDataAsync_WithValidInputs_ReturnsFilteredData()
    {
        // Arrange
        var allData = new List<GwpData>
        {
            new() { Country = "ae", Lob = "transport", Year = 2008, Premium = 400000000 },
            new() { Country = "ae", Lob = "property", Year = 2008, Premium = 1000000 },
            new() { Country = "us", Lob = "transport", Year = 2008, Premium = 100000000 }
        };

        _mockRepository
            .Setup(r => r.GetGwpDataAsync("ae", It.IsAny<List<string>>()))
            .ReturnsAsync(new List<GwpData> { allData[0] });

        // Act
        var result = await _mockRepository.Object.GetGwpDataAsync("ae", new List<string> { "transport" });

        // Assert
        Assert.Single(result);
        Assert.Equal("ae", result[0].Country);
        Assert.Equal("transport", result[0].Lob);
    }

    [Fact]
    public async Task GetGwpDataAsync_WithMultipleLobs_ReturnsAllMatching()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetGwpDataAsync("ae", It.IsAny<List<string>>()))
            .ReturnsAsync(new List<GwpData>
            {
                new() { Country = "ae", Lob = "transport", Year = 2008, Premium = 400000000 },
                new() { Country = "ae", Lob = "property", Year = 2008, Premium = 1000000 }
            });

        // Act
        var result = await _mockRepository.Object.GetGwpDataAsync("ae", new List<string> { "transport", "property" });

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetGwpDataAsync_NoMatches_ReturnsEmpty()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetGwpDataAsync("invalid", It.IsAny<List<string>>()))
            .ReturnsAsync(new List<GwpData>());

        // Act
        var result = await _mockRepository.Object.GetGwpDataAsync("invalid", new List<string> { "liability" });

        // Assert
        Assert.Empty(result);
    }
}
