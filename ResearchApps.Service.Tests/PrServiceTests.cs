namespace ResearchApps.Service.Tests;

public class PrServiceTests
{
    private readonly Mock<IPrRepo> _prRepoMock;
    private readonly Mock<IPrLineRepo> _prLineRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly PrService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public PrServiceTests()
    {
        _prRepoMock = new Mock<IPrRepo>();
        _prLineRepoMock = new Mock<IPrLineRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<PrService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new PrService(
            _prRepoMock.Object,
            _prLineRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task PrSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var prs = new PagedList<Pr>(
            new List<Pr>
            {
                new() { RecId = 1, PrId = "PR001", PrName = "PR 1" },
                new() { RecId = 2, PrId = "PR002", PrName = "PR 2" }
            },
            1,
            10,
            2
        );

        _prRepoMock
            .Setup(x => x.PrSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(prs);

        var result = await _sut.PrSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PRs retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task PrSelectById_WithValidId_ReturnsPr()
    {
        var prRecId = 1;
        var pr = new Pr { RecId = prRecId, PrId = "PR001", PrName = "Test PR" };

        _prRepoMock
            .Setup(x => x.PrSelectById(prRecId, _ct))
            .ReturnsAsync(pr);

        var result = await _sut.PrSelectById(prRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(prRecId, data.RecId);
    }

    [Fact]
    public async Task PrInsert_WithValidPr_ReturnsInsertedId()
    {
        var prVm = new PrVm { PrName = "New PR", PrDate = DateTime.Now };
        var insertedId = 10;

        _prRepoMock
            .Setup(x => x.PrInsert(It.IsAny<Pr>(), _ct))
            .ReturnsAsync(insertedId);

        var result = await _sut.PrInsert(prVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertedId, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrInsert_SetsCreatedByFromUserClaim()
    {
        var prVm = new PrVm { PrName = "New PR" };
        Pr? capturedPr = null;

        _prRepoMock
            .Setup(x => x.PrInsert(It.IsAny<Pr>(), _ct))
            .Callback<Pr, CancellationToken>((p, _) => capturedPr = p)
            .ReturnsAsync(1);

        await _sut.PrInsert(prVm, _ct);

        Assert.NotNull(capturedPr);
        Assert.Equal(_userClaimDto.Username, capturedPr.CreatedBy);
    }

    [Fact]
    public async Task PrUpdate_WithValidPr_CommitsTransaction()
    {
        var prVm = new PrVm { RecId = 1, PrName = "Updated PR" };

        _prRepoMock
            .Setup(x => x.PrUpdate(It.IsAny<Pr>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PrUpdate(prVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrUpdate_SetsModifiedByFromUserClaim()
    {
        var prVm = new PrVm { RecId = 1, PrName = "Updated PR" };
        Pr? capturedPr = null;

        _prRepoMock
            .Setup(x => x.PrUpdate(It.IsAny<Pr>(), _ct))
            .Callback<Pr, CancellationToken>((p, _) => capturedPr = p)
            .Returns(Task.CompletedTask);

        await _sut.PrUpdate(prVm, _ct);

        Assert.NotNull(capturedPr);
        Assert.Equal(_userClaimDto.Username, capturedPr.ModifiedBy);
    }

    [Fact]
    public async Task PrDelete_WithValidId_CommitsTransaction()
    {
        var prRecId = 1;

        _prRepoMock
            .Setup(x => x.PrDelete(prRecId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PrDelete(prRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrSubmitById_WithValidId_ReturnsSuccess()
    {
        var prRecId = 1;

        _prRepoMock
            .Setup(x => x.PrSubmitById(prRecId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PrSubmitById(prRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR submitted for approval successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrApproveById_WithValidAction_CommitsTransaction()
    {
        var action = new PrWorkflowActionVm { RecId = 1, Notes = "Approved" };

        _prRepoMock
            .Setup(x => x.PrApproveById(action.RecId, action.Notes, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PrApproveById(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR approved successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrApproveById_WithNullNotes_UsesEmptyString()
    {
        var action = new PrWorkflowActionVm { RecId = 1, Notes = null };
        string? capturedNotes = null;

        _prRepoMock
            .Setup(x => x.PrApproveById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), _ct))
            .Callback<int, string, string, CancellationToken>((_, notes, _, _) => capturedNotes = notes)
            .Returns(Task.CompletedTask);

        await _sut.PrApproveById(action, _ct);

        Assert.Equal(string.Empty, capturedNotes);
    }

    [Fact]
    public async Task PrRejectById_WithValidAction_CommitsTransaction()
    {
        var action = new PrWorkflowActionVm { RecId = 1, Notes = "Rejected due to..." };

        _prRepoMock
            .Setup(x => x.PrRejectById(action.RecId, action.Notes, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PrRejectById(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR rejected successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrRecallById_WithValidId_CommitsTransaction()
    {
        var prRecId = 1;

        _prRepoMock
            .Setup(x => x.PrRecallById(prRecId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PrRecallById(prRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR recalled successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var prVm = new PrVm { PrName = "New PR" };

        _prRepoMock
            .Setup(x => x.PrInsert(It.IsAny<Pr>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.PrInsert(prVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PrSubmitById_PassesCorrectUsernameToRepo()
    {
        var prRecId = 1;
        string? capturedUsername = null;

        _prRepoMock
            .Setup(x => x.PrSubmitById(It.IsAny<int>(), It.IsAny<string>(), _ct))
            .Callback<int, string, CancellationToken>((_, username, _) => capturedUsername = username)
            .Returns(Task.CompletedTask);

        await _sut.PrSubmitById(prRecId, _ct);

        Assert.Equal(_userClaimDto.Username, capturedUsername);
    }

    [Fact]
    public async Task GetPurchaseRequisition_WithValidId_ReturnsCompositeVm()
    {
        const int recId = 1;
        var pr = new Pr { RecId = recId, PrId = "PR001", PrName = "Test PR" };
        var lines = new List<PrLine>
        {
            new() { PrLineId = 1, PrId = "PR001", ItemId = 10 },
            new() { PrLineId = 2, PrId = "PR001", ItemId = 20 }
        };

        _prRepoMock.Setup(x => x.PrSelectById(recId, _ct)).ReturnsAsync(pr);
        _prLineRepoMock.Setup(x => x.PrLineSelectByPr("PR001", _ct)).ReturnsAsync(lines);

        var result = await _sut.GetPurchaseRequisition(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PR Composite ViewModel retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Header);
        Assert.NotNull(result.Data.Lines);
        Assert.Equal(2, result.Data.Lines.Count);
    }

    [Fact]
    public async Task PrSubmitById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var prRecId = 1;

        _prRepoMock
            .Setup(x => x.PrSubmitById(prRecId, _userClaimDto.Username, _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("No workflow configured"));

        var result = await _sut.PrSubmitById(prRecId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PrApproveById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new PrWorkflowActionVm { RecId = 1, Notes = "Approved" };

        _prRepoMock
            .Setup(x => x.PrApproveById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Approval not allowed"));

        var result = await _sut.PrApproveById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PrRejectById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new PrWorkflowActionVm { RecId = 1, Notes = "Rejected" };

        _prRepoMock
            .Setup(x => x.PrRejectById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Cannot reject"));

        var result = await _sut.PrRejectById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PrRecallById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var prRecId = 1;

        _prRepoMock
            .Setup(x => x.PrRecallById(prRecId, _userClaimDto.Username, _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Cannot recall"));

        var result = await _sut.PrRecallById(prRecId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetWfHistory_WithValidRefId_ReturnsHistory()
    {
        var refId = "PR001";
        var wfFormId = 3;
        var historyItems = new List<WfTransHistory>
        {
            new() { WfTransId = 1, RefId = refId, ActionDate = DateTime.Now, Notes = "Submitted" }
        };

        _prRepoMock
            .Setup(x => x.WfTransSelectByRefId(refId, wfFormId, _ct))
            .ReturnsAsync(historyItems);

        var result = await _sut.GetWfHistory(refId, wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow history retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }
}
