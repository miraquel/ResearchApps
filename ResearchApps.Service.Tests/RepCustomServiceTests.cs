namespace ResearchApps.Service.Tests;

public class RepCustomServiceTests
{
    private readonly Mock<IRepCustomRepo> _repCustomRepoMock;
    private readonly RepCustomService _sut;

    public RepCustomServiceTests()
    {
        _repCustomRepoMock = new Mock<IRepCustomRepo>();
        var loggerMock = new Mock<ILogger<RepCustomService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new RepCustomService(
            _repCustomRepoMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task RepTools_WithValidParams_ReturnsSuccess()
    {
        var year = 2026;
        var month = 3;
        var data = new List<RepTools> { new(), new() };

        _repCustomRepoMock
            .Setup(x => x.RepTools(year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _sut.RepTools(year, month, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<RepToolsVm>>>(result);
        Assert.NotNull(typed.Data);
    }

    [Fact]
    public async Task RepTools_WhenRepoThrowsException_ReturnsFailure500()
    {
        _repCustomRepoMock
            .Setup(x => x.RepTools(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _sut.RepTools(2026, 3, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors!);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task RepToolsAnalysis_WithValidParams_ReturnsSuccess()
    {
        var year = 2026;
        var month = 3;
        var data = new List<RepToolsAnalysis> { new(), new() };

        _repCustomRepoMock
            .Setup(x => x.RepToolsAnalysis(year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _sut.RepToolsAnalysis(year, month, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<RepToolsAnalysisVm>>>(result);
        Assert.NotNull(typed.Data);
    }

    [Fact]
    public async Task RepToolsAnalysis_WhenRepoThrowsException_ReturnsFailure500()
    {
        _repCustomRepoMock
            .Setup(x => x.RepToolsAnalysis(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _sut.RepToolsAnalysis(2026, 3, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors!);
        Assert.Equal(500, result.StatusCode);
    }
}
