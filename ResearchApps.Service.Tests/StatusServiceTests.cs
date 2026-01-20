namespace ResearchApps.Service.Tests;

public class StatusServiceTests
{
    private readonly Mock<IStatusRepo> _statusRepoMock;
    private readonly Mock<ILogger<StatusService>> _loggerMock;
    private readonly StatusService _sut;

    public StatusServiceTests()
    {
        _statusRepoMock = new Mock<IStatusRepo>();
        _loggerMock = new Mock<ILogger<StatusService>>();
        _sut = new StatusService(_statusRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task StatusCboAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "Active" };
        var cancellationToken = CancellationToken.None;
        var statuses = new List<Status>
        {
            new() { StatusId = 1, StatusName = "Active" },
            new() { StatusId = 2, StatusName = "Inactive" }
        };

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(statuses);

        // Act
        var result = await _sut.StatusCboAsync(cboRequest, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Statuses retrieved successfully.", result.Message);
        _statusRepoMock.Verify(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StatusCboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "NonExistent" };
        var cancellationToken = CancellationToken.None;

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.StatusCboAsync(cboRequest, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<StatusVm>>>(result);
        var data = Assert.IsType<IEnumerable<StatusVm>>(typed.Data, exactMatch: false);

        Assert.NotNull(data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task StatusCboAsync_WhenRepoThrowsException_PropagatesException()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "Status" };
        var cancellationToken = CancellationToken.None;

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.StatusCboAsync(cboRequest, cancellationToken));
    }

    [Fact]
    public async Task StatusCboAsync_WithNullTerm_StillCallsRepo()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = null };
        var cancellationToken = CancellationToken.None;

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.StatusCboAsync(cboRequest, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        _statusRepoMock.Verify(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StatusCboAsync_PassesCancellationTokenToRepo()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "Status" };
        var cancellationToken = new CancellationTokenSource().Token;

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync([]);

        // Act
        await _sut.StatusCboAsync(cboRequest, cancellationToken);

        // Assert
        _statusRepoMock.Verify(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StatusCboAsync_MapsEntityToVmCorrectly()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "Test" };
        var cancellationToken = CancellationToken.None;
        var statuses = new List<Status>
        {
            new() { StatusId = 1, StatusName = "Test Status" }
        };

        _statusRepoMock
            .Setup(x => x.StatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(statuses);

        // Act
        var result = await _sut.StatusCboAsync(cboRequest, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<StatusVm>>>(result);
        var data = Assert.IsType<IEnumerable<StatusVm>>(typed.Data, exactMatch: false);
        Assert.NotNull(data);
        var statusVm = data.First();
        Assert.Equal(1, statusVm.StatusId);
        Assert.Equal("Test Status", statusVm.StatusName);
    }
}




