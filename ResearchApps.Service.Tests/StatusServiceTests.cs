namespace ResearchApps.Service.Tests;

public class StatusServiceTests
{
    private readonly Mock<IStatusRepo> _statusRepoMock;
    private readonly StatusService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public StatusServiceTests()
    {
        _statusRepoMock = new Mock<IStatusRepo>();
        var loggerMock = new Mock<ILogger<StatusService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _sut = new StatusService(_statusRepoMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task StatusCboAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        var cboRequest = new CboRequestVm { Term = "Active" };
        var statuses = new List<Status>
        {
            new() { StatusId = 1, StatusName = "Active" },
            new() { StatusId = 2, StatusName = "Inactive" }
        };

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(statuses);

        var result = await _sut.StatusCboAsync(cboRequest, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Statuses retrieved successfully.", result.Message);
        _statusRepoMock.Verify(x => x.StatusCboAsync(It.IsAny<CboRequest>(), _ct), Times.Once);
    }

    [Fact]
    public async Task StatusCboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        var cboRequest = new CboRequestVm { Term = "NonExistent" };

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync([]);

        var result = await _sut.StatusCboAsync(cboRequest, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<StatusVm>>>(result);
        var data = Assert.IsType<IEnumerable<StatusVm>>(typed.Data, exactMatch: false);

        Assert.NotNull(data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task StatusCboAsync_WhenRepoThrowsException_PropagatesException()
    {
        var cboRequest = new CboRequestVm { Term = "Status" };

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.StatusCboAsync(cboRequest, _ct));
    }

    [Fact]
    public async Task StatusCboAsync_WithNullTerm_StillCallsRepo()
    {
        var cboRequest = new CboRequestVm { Term = null };

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync([]);

        var result = await _sut.StatusCboAsync(cboRequest, _ct);

        Assert.True(result.IsSuccess);
        _statusRepoMock.Verify(x => x.StatusCboAsync(It.IsAny<CboRequest>(), _ct), Times.Once);
    }

    [Fact]
    public async Task StatusCboAsync_PassesCancellationTokenToRepo()
    {
        var cboRequest = new CboRequestVm { Term = "Status" };
        var cancellationToken = new CancellationTokenSource().Token;

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync([]);

        await _sut.StatusCboAsync(cboRequest, cancellationToken);

        _statusRepoMock.Verify(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StatusCboAsync_MapsEntityToVmCorrectly()
    {
        var cboRequest = new CboRequestVm { Term = "Test" };
        var statuses = new List<Status>
        {
            new() { StatusId = 1, StatusName = "Test Status" }
        };

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(statuses);

        var result = await _sut.StatusCboAsync(cboRequest, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<StatusVm>>>(result);
        var data = Assert.IsType<IEnumerable<StatusVm>>(typed.Data, exactMatch: false);
        Assert.NotNull(data);
        var statusVm = data.First();
        Assert.Equal(1, statusVm.StatusId);
        Assert.Equal("Test Status", statusVm.StatusName);
    }
}
