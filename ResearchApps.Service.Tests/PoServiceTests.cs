namespace ResearchApps.Service.Tests;

public class PoServiceTests
{
    private readonly Mock<IPoRepo> _poRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly PoService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public PoServiceTests()
    {
        _poRepoMock = new Mock<IPoRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<PoService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new PoService(
            _poRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task PoSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var poList = new List<Po>
        {
            new() { RecId = 1, PoId = "PO001", SupplierName = "Test Supplier 1" },
            new() { RecId = 2, PoId = "PO002", SupplierName = "Test Supplier 2" }
        };

        _poRepoMock
            .Setup(x => x.PoSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(new PagedList<Po>(poList, 1, 10, 2));

        var result = await _sut.PoSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());
    }

    [Fact]
    public async Task PoSelectById_WithExistingId_ReturnsPo()
    {
        var recId = 1;
        var po = new Po { RecId = recId, PoId = "PO001", SupplierName = "Test Supplier" };

        _poRepoMock
            .Setup(x => x.PoSelectById(recId, _ct))
            .ReturnsAsync(po);

        var result = await _sut.PoSelectById(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("PO001", result.Data.Header.PoId);
    }

    [Fact]
    public async Task PoSelectById_WithNonExistingId_ReturnsNotFound()
    {
        var recId = 999;

        _poRepoMock
            .Setup(x => x.PoSelectById(recId, _ct))
            .ReturnsAsync((Po?)null);

        var result = await _sut.PoSelectById(recId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task PoInsert_WithValidData_ReturnsCreatedAndCommits()
    {
        var poVm = new PoHeaderVm
        {
            PoDate = DateTime.Now,
            SupplierId = 1
        };

        var insertedPo = new Po
        {
            RecId = 1,
            PoId = "PO001",
            SupplierId = 1,
            CreatedBy = "testuser"
        };

        _poRepoMock
            .Setup(x => x.PoInsert(It.IsAny<Po>(), _ct))
            .ReturnsAsync(insertedPo);

        var result = await _sut.PoInsert(poVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("PO001", result.Data.Header.PoId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoUpdate_WithValidData_ReturnsUpdatedAndCommits()
    {
        var poVm = new PoHeaderVm
        {
            RecId = 1,
            PoId = "PO001",
            PoDate = DateTime.Now,
            SupplierId = 1
        };

        var updatedPo = new Po
        {
            RecId = 1,
            PoId = "PO001",
            SupplierName = "Updated Supplier",
            ModifiedBy = "testuser"
        };

        _poRepoMock
            .Setup(x => x.PoUpdate(It.IsAny<Po>(), _ct))
            .ReturnsAsync(updatedPo);

        var result = await _sut.PoUpdate(poVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Updated Supplier", result.Data.Header.SupplierName);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoDelete_WithValidId_ReturnsSuccessAndCommits()
    {
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoDelete(recId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PoDelete(recId, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoSubmitById_WithValidId_ReturnsSuccess()
    {
        var recId = 1;
        var submittedPo = new Po
        {
            RecId = recId,
            PoId = "PO001",
            PoStatusId = 4,
            CurrentApprover = "approver1"
        };

        _poRepoMock
            .Setup(x => x.PoSubmitById(recId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        _poRepoMock
            .Setup(x => x.PoSelectById(recId, _ct))
            .ReturnsAsync(submittedPo);

        var result = await _sut.PoSubmitById(recId, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoRecallById_WithValidId_ReturnsSuccessAndCommits()
    {
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoRecallById(recId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PoRecallById(recId, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoApproveById_WithValidAction_ReturnsSuccessAndCommits()
    {
        var action = new PoWorkflowActionVm { RecId = 1, Notes = "Approved" };

        _poRepoMock
            .Setup(x => x.PoApproveById(action.RecId, action.Notes, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PoApproveById(action, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoRejectById_WithValidAction_ReturnsSuccessAndCommits()
    {
        var action = new PoWorkflowActionVm { RecId = 1, Notes = "Rejected - insufficient budget" };

        _poRepoMock
            .Setup(x => x.PoRejectById(action.RecId, action.Notes, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PoRejectById(action, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoCloseById_WithValidId_ReturnsSuccessAndCommits()
    {
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoCloseById(recId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PoCloseById(recId, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoOsSelect_WithValidSupplierId_ReturnsOutstandingHeaders()
    {
        var supplierId = 1;
        var osHeaders = new List<PoHeaderOutstanding>
        {
            new() { RecId = 1, PoId = "PO001", SupplierId = supplierId },
            new() { RecId = 2, PoId = "PO002", SupplierId = supplierId }
        };

        _poRepoMock
            .Setup(x => x.PoOsSelect(supplierId, _ct))
            .ReturnsAsync(osHeaders);

        var result = await _sut.PoOsSelect(supplierId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task PoOsSelectById_WithValidPoLineId_ReturnsOutstandingLines()
    {
        var poLineId = 1;
        var osLines = new List<PoLineOutstanding>
        {
            new() { PoLineId = 1, PoId = "PO001", ItemId = 1, QtyOs = 10 },
            new() { PoLineId = 2, PoId = "PO001", ItemId = 2, QtyOs = 5 }
        };

        _poRepoMock
            .Setup(x => x.PoOsSelectById(poLineId, _ct))
            .ReturnsAsync(osLines);

        var result = await _sut.PoOsSelectById(poLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task PoSubmitById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoSubmitById(recId, _userClaimDto.Username, _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("No workflow configured"));

        var result = await _sut.PoSubmitById(recId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PoRecallById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoRecallById(recId, _userClaimDto.Username, _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Cannot recall at this stage"));

        var result = await _sut.PoRecallById(recId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PoApproveById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new PoWorkflowActionVm { RecId = 1, Notes = "Approved" };

        _poRepoMock
            .Setup(x => x.PoApproveById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Approval not allowed"));

        var result = await _sut.PoApproveById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PoRejectById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new PoWorkflowActionVm { RecId = 1, Notes = "Rejected" };

        _poRepoMock
            .Setup(x => x.PoRejectById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Cannot reject at this stage"));

        var result = await _sut.PoRejectById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PoCloseById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoCloseById(recId, _userClaimDto.Username, _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Cannot close at this stage"));

        var result = await _sut.PoCloseById(recId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetWfHistory_WithValidRefId_ReturnsHistory()
    {
        var refId = "PO001";
        var wfFormId = 2;
        var historyItems = new List<WfTransHistory>
        {
            new() { WfTransId = 1, RefId = refId, ActionDate = DateTime.Now, Notes = "Submitted" },
            new() { WfTransId = 2, RefId = refId, ActionDate = DateTime.Now, Notes = "Approved" }
        };

        _poRepoMock
            .Setup(x => x.WfTransSelectByRefId(refId, wfFormId, _ct))
            .ReturnsAsync(historyItems);

        var result = await _sut.GetWfHistory(refId, wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow history retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }
}
