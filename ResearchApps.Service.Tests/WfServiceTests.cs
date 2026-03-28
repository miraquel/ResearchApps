namespace ResearchApps.Service.Tests;

public class WfServiceTests
{
    private readonly Mock<IWfRepo> _wfRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly WfService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public WfServiceTests()
    {
        _wfRepoMock = new Mock<IWfRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser", UserId = Guid.NewGuid() };
        var loggerMock = new Mock<ILogger<WfService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new WfService(
            _wfRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task WfSelectByWfFormIdAsync_WithValidId_ReturnsMappedSteps()
    {
        const int wfFormId = 4;
        var steps = new List<Wf>
        {
            new() { WfId = 1, WfFormId = wfFormId, Index = 1, UserId = "approver1" },
            new() { WfId = 2, WfFormId = wfFormId, Index = 2, UserId = "approver2" }
        };

        _wfRepoMock
            .Setup(x => x.WfSelectByWfFormIdAsync(wfFormId, _ct))
            .ReturnsAsync(steps);

        var result = await _sut.WfSelectByWfFormIdAsync(wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Approval steps retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<WfVm>>>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<WfVm>>(typed.Data).ToList();
        Assert.Equal(2, data.Count);
        Assert.Equal("approver1", data[0].UserId);
        _wfRepoMock.Verify(x => x.WfSelectByWfFormIdAsync(wfFormId, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfSelectByWfFormIdAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        const int wfFormId = 99;

        _wfRepoMock
            .Setup(x => x.WfSelectByWfFormIdAsync(wfFormId, _ct))
            .ReturnsAsync([]);

        var result = await _sut.WfSelectByWfFormIdAsync(wfFormId, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<WfVm>>>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<WfVm>>(typed.Data);
        Assert.Empty(data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfSelectByIdAsync_WithValidId_ReturnsMappedStep()
    {
        const int wfId = 10;
        var wf = new Wf { WfId = wfId, WfFormId = 4, Index = 1, UserId = "approver1" };

        _wfRepoMock
            .Setup(x => x.WfSelectByIdAsync(wfId, _ct))
            .ReturnsAsync(wf);

        var result = await _sut.WfSelectByIdAsync(wfId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Approval step retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<WfVm>>(result);
        var data = Assert.IsType<WfVm>(typed.Data);
        Assert.Equal(wfId, data.WfId);
        Assert.Equal("approver1", data.UserId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfInsertAsync_WithValidInput_ReturnsCreatedAndCommits()
    {
        var wfVm = new WfVm { WfFormId = 4, Index = 1, UserId = "approver1" };
        Wf? capturedWf = null;
        var inserted = new Wf { WfId = 25, WfFormId = 4, Index = 1, UserId = "approver1" };

        _wfRepoMock
            .Setup(x => x.WfInsertAsync(It.IsAny<Wf>(), _ct))
            .Callback<Wf, CancellationToken>((wf, _) => capturedWf = wf)
            .ReturnsAsync(inserted);

        var result = await _sut.WfInsertAsync(wfVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Approval step created successfully.", result.Message);
        Assert.NotNull(capturedWf);
        Assert.Equal(wfVm.WfFormId, capturedWf.WfFormId);
        Assert.Equal(wfVm.Index, capturedWf.Index);
        Assert.Equal(wfVm.UserId, capturedWf.UserId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task WfInsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var wfVm = new WfVm { WfFormId = 4, Index = 1, UserId = "approver1" };

        _wfRepoMock
            .Setup(x => x.WfInsertAsync(It.IsAny<Wf>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.WfInsertAsync(wfVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfUpdateAsync_WithValidInput_ReturnsUpdatedAndCommits()
    {
        var wfVm = new WfVm { WfId = 25, WfFormId = 4, Index = 2, UserId = "approver2" };
        Wf? capturedWf = null;
        var updated = new Wf { WfId = 25, WfFormId = 4, Index = 2, UserId = "approver2" };

        _wfRepoMock
            .Setup(x => x.WfUpdateAsync(It.IsAny<Wf>(), _ct))
            .Callback<Wf, CancellationToken>((wf, _) => capturedWf = wf)
            .ReturnsAsync(updated);

        var result = await _sut.WfUpdateAsync(wfVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Approval step updated successfully.", result.Message);
        Assert.NotNull(capturedWf);
        Assert.Equal(wfVm.WfId, capturedWf.WfId);
        Assert.Equal(wfVm.Index, capturedWf.Index);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task WfUpdateAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var wfVm = new WfVm { WfId = 25, WfFormId = 4, Index = 2, UserId = "approver2" };

        _wfRepoMock
            .Setup(x => x.WfUpdateAsync(It.IsAny<Wf>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.WfUpdateAsync(wfVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task WfDeleteAsync_WithValidId_ReturnsSuccessAndCommits()
    {
        const int wfId = 25;

        _wfRepoMock
            .Setup(x => x.WfDeleteAsync(wfId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.WfDeleteAsync(wfId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Approval step deleted successfully.", result.Message);
        _wfRepoMock.Verify(x => x.WfDeleteAsync(wfId, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task WfDeleteAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        const int wfId = 25;

        _wfRepoMock
            .Setup(x => x.WfDeleteAsync(wfId, _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.WfDeleteAsync(wfId, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
