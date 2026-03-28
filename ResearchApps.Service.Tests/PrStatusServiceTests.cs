namespace ResearchApps.Service.Tests;

public class PrStatusServiceTests
{
    private readonly Mock<IPrStatusRepo> _prStatusRepoMock;
    private readonly PrStatusService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public PrStatusServiceTests()
    {
        _prStatusRepoMock = new Mock<IPrStatusRepo>();
        var loggerMock = new Mock<ILogger<PrStatusService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _sut = new PrStatusService(_prStatusRepoMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task PrStatusCboAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        var request = new CboRequestVm { Term = "Pending" };
        var prStatuses = new List<PrStatus>
        {
            new() { PrStatusId = 1, PrStatusName = "Draft" },
            new() { PrStatusId = 2, PrStatusName = "Submitted" }
        };

        _prStatusRepoMock
            .Setup(x => x.PrStatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(prStatuses);

        var result = await _sut.PrStatusCboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PrStatus Cbo fetched successfully.", result.Message);
    }

    [Fact]
    public async Task PrStatusCboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        var request = new CboRequestVm { Term = "NonExistent" };

        _prStatusRepoMock
            .Setup(x => x.PrStatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(new List<PrStatus>());

        var result = await _sut.PrStatusCboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<PrStatusVm>>>(result);
        var data = Assert.IsType<IEnumerable<PrStatusVm>>(typed.Data, exactMatch: false);
        Assert.NotNull(data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task PrStatusCboAsync_WhenRepoThrowsException_PropagatesException()
    {
        var request = new CboRequestVm { Term = "Status" };

        _prStatusRepoMock
            .Setup(x => x.PrStatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.PrStatusCboAsync(request, _ct));
    }
}
