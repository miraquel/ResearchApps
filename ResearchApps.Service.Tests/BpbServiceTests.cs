namespace ResearchApps.Service.Tests;

public class BpbServiceTests
{
    private readonly Mock<IBpbRepo> _bpbRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly BpbService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public BpbServiceTests()
    {
        _bpbRepoMock = new Mock<IBpbRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<BpbService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new BpbService(
            _bpbRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task BpbSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var bpbs = new PagedList<BpbHeader>(
            new List<BpbHeader>
            {
                new() { RecId = 1, BpbId = "BPB2501", Amount = 100 },
                new() { RecId = 2, BpbId = "BPB2502", Amount = 200 }
            }, 1, 10, 2);

        _bpbRepoMock
            .Setup(x => x.BpbSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(bpbs);

        var result = await _sut.BpbSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPBs retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task BpbSelectById_WithExistingId_ReturnsBpb()
    {
        var bpb = new BpbHeader { RecId = 1, BpbId = "BPB2501" };

        _bpbRepoMock
            .Setup(x => x.BpbSelectById(1, _ct))
            .ReturnsAsync(bpb);

        var result = await _sut.BpbSelectById(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPB retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("BPB2501", result.Data.BpbId);
    }

    [Fact]
    public async Task BpbSelectById_WithNonExistentId_ReturnsNotFound()
    {
        _bpbRepoMock
            .Setup(x => x.BpbSelectById(999, _ct))
            .ReturnsAsync((BpbHeader?)null);

        var result = await _sut.BpbSelectById(999, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetBpb_WithExistingId_ReturnsHeaderAndLines()
    {
        var header = new BpbHeader { RecId = 1, BpbId = "BPB2501" };
        var lines = new List<BpbLine>
        {
            new() { BpbLineId = 1, BpbRecId = 1, ItemId = 10, Qty = 5 }
        };

        _bpbRepoMock.Setup(x => x.BpbSelectById(1, _ct)).ReturnsAsync(header);
        _bpbRepoMock.Setup(x => x.BpbLineSelectByBpb(1, _ct)).ReturnsAsync(lines);

        var result = await _sut.GetBpb(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Header);
        Assert.NotNull(result.Data.Lines);
    }

    [Fact]
    public async Task GetBpb_WithNonExistentId_ReturnsNotFound()
    {
        _bpbRepoMock.Setup(x => x.BpbSelectById(999, _ct)).ReturnsAsync((BpbHeader?)null);

        var result = await _sut.GetBpb(999, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task BpbSelectByProd_WithValidProdId_ReturnsBpbs()
    {
        var bpbs = new List<BpbHeader>
        {
            new() { RecId = 1, BpbId = "BPB2501", RefId = "PROD001" }
        };

        _bpbRepoMock.Setup(x => x.BpbSelectByProd("PROD001", _ct)).ReturnsAsync(bpbs);

        var result = await _sut.BpbSelectByProd("PROD001", _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPBs retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task BpbInsert_WithValidData_ReturnsCreatedAndCommits()
    {
        var headerVm = new BpbHeaderVm { BpbDate = DateTime.Now, RefId = "PROD001", Descr = "Test" };

        _bpbRepoMock
            .Setup(x => x.BpbInsert(It.IsAny<BpbHeader>(), _ct))
            .ReturnsAsync(42);

        var result = await _sut.BpbInsert(headerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(42, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BpbInsert_SetsCreatedByFromUserClaim()
    {
        var headerVm = new BpbHeaderVm { BpbDate = DateTime.Now, RefId = "PROD001" };
        BpbHeader? captured = null;

        _bpbRepoMock
            .Setup(x => x.BpbInsert(It.IsAny<BpbHeader>(), _ct))
            .Callback<BpbHeader, CancellationToken>((h, _) => captured = h)
            .ReturnsAsync(1);

        await _sut.BpbInsert(headerVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task BpbUpdate_WithValidData_CommitsTransaction()
    {
        var headerVm = new BpbHeaderVm { RecId = 1, BpbId = "BPB2501", BpbDate = DateTime.Now };

        _bpbRepoMock
            .Setup(x => x.BpbUpdate(It.IsAny<BpbHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.BpbUpdate(headerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPB updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BpbUpdate_SetsModifiedByFromUserClaim()
    {
        var headerVm = new BpbHeaderVm { RecId = 1, BpbId = "BPB2501" };
        BpbHeader? captured = null;

        _bpbRepoMock
            .Setup(x => x.BpbUpdate(It.IsAny<BpbHeader>(), _ct))
            .Callback<BpbHeader, CancellationToken>((h, _) => captured = h)
            .Returns(Task.CompletedTask);

        await _sut.BpbUpdate(headerVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.ModifiedBy);
    }

    [Fact]
    public async Task BpbDelete_WithValidId_CommitsTransaction()
    {
        _bpbRepoMock
            .Setup(x => x.BpbDelete(1, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.BpbDelete(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPB deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BpbLineSelectByBpb_ReturnsLines()
    {
        var lines = new List<BpbLine>
        {
            new() { BpbLineId = 1, BpbRecId = 1, ItemId = 10 },
            new() { BpbLineId = 2, BpbRecId = 1, ItemId = 20 }
        };

        _bpbRepoMock.Setup(x => x.BpbLineSelectByBpb(1, _ct)).ReturnsAsync(lines);

        var result = await _sut.BpbLineSelectByBpb(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPB lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task BpbLineSelectById_WithExistingId_ReturnsLine()
    {
        var line = new BpbLine { BpbLineId = 1, BpbRecId = 1, ItemId = 10, Qty = 5 };

        _bpbRepoMock.Setup(x => x.BpbLineSelectById(1, _ct)).ReturnsAsync(line);

        var result = await _sut.BpbLineSelectById(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.BpbLineId);
    }

    [Fact]
    public async Task BpbLineSelectById_WithNonExistentId_ReturnsNotFound()
    {
        _bpbRepoMock.Setup(x => x.BpbLineSelectById(999, _ct)).ReturnsAsync((BpbLine?)null);

        var result = await _sut.BpbLineSelectById(999, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task BpbLineInsert_WithValidData_ReturnsCreatedAndCommits()
    {
        var lineVm = new BpbLineVm { BpbRecId = 1, ItemId = 10, Qty = 5 };

        _bpbRepoMock
            .Setup(x => x.BpbLineInsert(It.IsAny<BpbLine>(), _ct))
            .ReturnsAsync(100);

        var result = await _sut.BpbLineInsert(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(100, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BpbLineInsert_SetsCreatedByFromUserClaim()
    {
        var lineVm = new BpbLineVm { BpbRecId = 1, ItemId = 10, Qty = 5 };
        BpbLine? captured = null;

        _bpbRepoMock
            .Setup(x => x.BpbLineInsert(It.IsAny<BpbLine>(), _ct))
            .Callback<BpbLine, CancellationToken>((l, _) => captured = l)
            .ReturnsAsync(1);

        await _sut.BpbLineInsert(lineVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task BpbLineUpdate_WithValidData_CommitsTransaction()
    {
        var lineVm = new BpbLineVm { BpbLineId = 1, BpbRecId = 1, ItemId = 10, Qty = 10 };

        _bpbRepoMock
            .Setup(x => x.BpbLineUpdate(It.IsAny<BpbLine>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.BpbLineUpdate(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPB line updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BpbLineDelete_WithValidId_CommitsTransaction()
    {
        _bpbRepoMock
            .Setup(x => x.BpbLineDelete(1, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.BpbLineDelete(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("BPB line deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CheckStock_WithSufficientStock_ReturnsSuccess()
    {
        _bpbRepoMock
            .Setup(x => x.GetStockInfo(10, 1, _ct))
            .ReturnsAsync((100m, 20m));

        var result = await _sut.CheckStock(10, 1, 50, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.IsAvailable);
        Assert.False(result.Data.WillBeBelowBuffer);
    }

    [Fact]
    public async Task CheckStock_WithInsufficientStock_ReturnsFailure()
    {
        _bpbRepoMock
            .Setup(x => x.GetStockInfo(10, 1, _ct))
            .ReturnsAsync((30m, 20m));

        var result = await _sut.CheckStock(10, 1, 50, _ct);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors!, e => e.Contains("Insufficient stock"));
    }

    [Fact]
    public async Task CheckStock_WhenStockWillBeBelowBuffer_ReturnsWarning()
    {
        _bpbRepoMock
            .Setup(x => x.GetStockInfo(10, 1, _ct))
            .ReturnsAsync((50m, 30m));

        var result = await _sut.CheckStock(10, 1, 25, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.IsAvailable);
        Assert.True(result.Data.WillBeBelowBuffer);
        Assert.Contains("below buffer", result.Data.Message);
    }

    [Fact]
    public async Task BpbInsert_WhenRepoThrows_DoesNotCommitTransaction()
    {
        var headerVm = new BpbHeaderVm { BpbDate = DateTime.Now, RefId = "PROD001" };

        _bpbRepoMock
            .Setup(x => x.BpbInsert(It.IsAny<BpbHeader>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.BpbInsert(headerVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task BpbLineInsert_WhenRepoThrows_DoesNotCommitTransaction()
    {
        var lineVm = new BpbLineVm { BpbRecId = 1, ItemId = 10, Qty = 5 };

        _bpbRepoMock
            .Setup(x => x.BpbLineInsert(It.IsAny<BpbLine>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.BpbLineInsert(lineVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
