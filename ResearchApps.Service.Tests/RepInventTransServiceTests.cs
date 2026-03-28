namespace ResearchApps.Service.Tests;

public class RepInventTransServiceTests
{
    private readonly Mock<IRepInventTransRepo> _repInventTransRepoMock;
    private readonly RepInventTransService _sut;

    public RepInventTransServiceTests()
    {
        _repInventTransRepoMock = new Mock<IRepInventTransRepo>();
        var loggerMock = new Mock<ILogger<RepInventTransService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new RepInventTransService(
            _repInventTransRepoMock.Object,
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
    public async Task RepInventTransByItem_WithDateRange_PassesDatesTORepo()
    {
        var itemId = 1;
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 3, 31);
        DateTime? capturedStart = null;
        DateTime? capturedEnd = null;

        _repInventTransRepoMock
            .Setup(x => x.RepInventTransByItem(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<int, DateTime?, DateTime?, CancellationToken>((_, s, e, _) =>
            {
                capturedStart = s;
                capturedEnd = e;
            })
            .ReturnsAsync(new List<RepInventTransByItem>());

        await _sut.RepInventTransByItem(itemId, startDate, endDate, CancellationToken.None);

        Assert.Equal(startDate, capturedStart);
        Assert.Equal(endDate, capturedEnd);
    }
}
