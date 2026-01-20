namespace ResearchApps.Service.Tests;

public class PrStatusServiceTests
{
    private readonly Mock<IPrStatusRepo> _prStatusRepoMock;
    private readonly Mock<ILogger<PrStatusService>> _loggerMock;
    private readonly PrStatusService _sut;

    public PrStatusServiceTests()
    {
        _prStatusRepoMock = new Mock<IPrStatusRepo>();
        _loggerMock = new Mock<ILogger<PrStatusService>>();
        _sut = new PrStatusService(_prStatusRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task PrStatusCboAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new CboRequestVm { Term = "Pending" };
        var cancellationToken = CancellationToken.None;
        var prStatuses = new List<PrStatus>
        {
            new() { PrStatusId = 1, PrStatusName = "Draft" },
            new() { PrStatusId = 2, PrStatusName = "Submitted" }
        };

        _prStatusRepoMock
            .Setup(x => x.PrStatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(prStatuses);

        // Act
        var result = await _sut.PrStatusCboAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PrStatus Cbo fetched successfully.", result.Message);
    }

    [Fact]
    public async Task PrStatusCboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        // Arrange
        var request = new CboRequestVm { Term = "NonExistent" };
        var cancellationToken = CancellationToken.None;

        _prStatusRepoMock
            .Setup(x => x.PrStatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(new List<PrStatus>());

        // Act
        var result = await _sut.PrStatusCboAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<PrStatusVm>>>(result);
        var data = Assert.IsType<IEnumerable<PrStatusVm>>(typed.Data, exactMatch: false);
        Assert.NotNull(data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task PrStatusCboAsync_WhenRepoThrowsException_PropagatesException()
    {
        // Arrange
        var request = new CboRequestVm { Term = "Status" };
        var cancellationToken = CancellationToken.None;

        _prStatusRepoMock
            .Setup(x => x.PrStatusCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.PrStatusCboAsync(request, cancellationToken));
    }
}




