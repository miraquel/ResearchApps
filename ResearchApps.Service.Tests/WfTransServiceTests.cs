namespace ResearchApps.Service.Tests;

public class WfTransServiceTests
{
    private readonly Mock<IWfTransRepo> _wfTransRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly WfTransService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public WfTransServiceTests()
    {
        _wfTransRepoMock = new Mock<IWfTransRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<WfTransService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new WfTransService(
            _wfTransRepoMock.Object,
            _dbTransactionMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task WfTransSelectByRefIdAsync_WithValidInput_ReturnsMappedHistory()
    {
        const string refId = "PR001";
        const int wfFormId = 4;

        var history = new List<WfTransHistory>
        {
            new()
            {
                WfTransId = 1,
                WfId = 10,
                WfFormId = wfFormId,
                FormName = "PR",
                RefId = refId,
                Index = 1,
                UserId = "approver1",
                WfStatusActionId = 5,
                WfStatusActionName = "Approved",
                ActionDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                Notes = "Looks good"
            },
            new()
            {
                WfTransId = 2,
                WfId = 10,
                WfFormId = wfFormId,
                FormName = "PR",
                RefId = refId,
                Index = 2,
                UserId = "approver2",
                WfStatusActionId = 6,
                WfStatusActionName = "Rejected",
                ActionDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                Notes = "Need updates"
            }
        };

        _wfTransRepoMock
            .Setup(x => x.WfTransSelectByRefIdAsync(refId, wfFormId, _ct))
            .ReturnsAsync(history);

        var result = await _sut.WfTransSelectByRefIdAsync(refId, wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow transactions retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<WfTransHistoryVm>>>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<WfTransHistoryVm>>(typed.Data).ToList();
        Assert.Equal(2, data.Count);
        Assert.Equal("PR001", data[0].RefId);
        _wfTransRepoMock.Verify(x => x.WfTransSelectByRefIdAsync(refId, wfFormId, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfTransSelectByRefIdAsync_WithNoHistory_ReturnsSuccessWithEmptyCollection()
    {
        const string refId = "PR999";
        const int wfFormId = 4;

        _wfTransRepoMock
            .Setup(x => x.WfTransSelectByRefIdAsync(refId, wfFormId, _ct))
            .ReturnsAsync([]);

        var result = await _sut.WfTransSelectByRefIdAsync(refId, wfFormId, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<WfTransHistoryVm>>>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<WfTransHistoryVm>>(typed.Data);
        Assert.Empty(data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfTransSelectByRefIdAsync_WhenRepoThrows_PropagatesException()
    {
        const string refId = "PR001";
        const int wfFormId = 4;

        _wfTransRepoMock
            .Setup(x => x.WfTransSelectByRefIdAsync(refId, wfFormId, _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.WfTransSelectByRefIdAsync(refId, wfFormId, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfTransSelectByRefIdAsync_PassesCancellationTokenToRepo()
    {
        const string refId = "PR001";
        const int wfFormId = 4;
        var cancellationToken = new CancellationToken(true);

        _wfTransRepoMock
            .Setup(x => x.WfTransSelectByRefIdAsync(refId, wfFormId, cancellationToken))
            .ThrowsAsync(new TaskCanceledException());

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _sut.WfTransSelectByRefIdAsync(refId, wfFormId, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
