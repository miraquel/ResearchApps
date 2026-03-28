namespace ResearchApps.Service.Tests;

public class RepStockServiceTests
{
    private readonly Mock<IRepInventTransRepo> _repInventTransRepoMock;
    private readonly Mock<IRepStockRepo> _repStockRepoMock;
    private readonly RepStockService _sut;

    public RepStockServiceTests()
    {
        _repInventTransRepoMock = new Mock<IRepInventTransRepo>();
        _repStockRepoMock = new Mock<IRepStockRepo>();
        var loggerMock = new Mock<ILogger<RepStockService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new RepStockService(
            _repInventTransRepoMock.Object,
            _repStockRepoMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task RepInventTransByItem_WithValidParams_ReturnsSuccess()
    {
        var itemId = 1;
        var data = new List<RepInventTransByItem> { new(), new() };

        _repInventTransRepoMock
            .Setup(x => x.RepInventTransByItem(itemId, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _sut.RepInventTransByItem(itemId, null, null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<RepInventTransByItemVm>>>(result);
        Assert.NotNull(typed.Data);
    }

    [Fact]
    public async Task RepInventTransByItem_WhenRepoThrowsException_ReturnsFailure500()
    {
        _repInventTransRepoMock
            .Setup(x => x.RepInventTransByItem(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _sut.RepInventTransByItem(1, null, null, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors!);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task RepStockCardMonthly_WithValidParams_ReturnsSuccess()
    {
        var itemId = 1;
        var year = 2026;
        var month = 3;
        var data = new List<RepStockCardMonthly> { new(), new() };

        _repStockRepoMock
            .Setup(x => x.RepStockCardMonthly(itemId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _sut.RepStockCardMonthly(itemId, year, month, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<RepStockCardMonthlyVm>>>(result);
        Assert.NotNull(typed.Data);
    }

    [Fact]
    public async Task RepStockCardMonthly_WhenRepoThrowsException_ReturnsFailure500()
    {
        _repStockRepoMock
            .Setup(x => x.RepStockCardMonthly(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _sut.RepStockCardMonthly(1, 2026, 3, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors!);
        Assert.Equal(500, result.StatusCode);
    }
}
