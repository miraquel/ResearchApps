namespace ResearchApps.Service.Tests;

public class GoodsReceiptServiceTests
{
    private readonly Mock<IGoodsReceiptRepo> _grRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly GoodsReceiptService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GoodsReceiptServiceTests()
    {
        _grRepoMock = new Mock<IGoodsReceiptRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<GoodsReceiptService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new GoodsReceiptService(
            _grRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task GrSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var headers = new List<GoodsReceiptHeader>
        {
            new() { RecId = 1, GrId = "GR001", SupplierId = 1 },
            new() { RecId = 2, GrId = "GR002", SupplierId = 2 }
        };

        _grRepoMock
            .Setup(x => x.GrSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(new PagedList<GoodsReceiptHeader>(headers, 1, 10, 2));

        var result = await _sut.GrSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());
    }

    [Fact]
    public async Task GrSelectById_WithValidId_ReturnsGr()
    {
        var recId = 1;
        var header = new GoodsReceiptHeader { RecId = recId, GrId = "GR001", SupplierId = 1 };

        _grRepoMock
            .Setup(x => x.GrSelectById(recId, _ct))
            .ReturnsAsync(header);

        var result = await _sut.GrSelectById(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("GR001", result.Data.GrId);
    }

    [Fact]
    public async Task GrInsert_WithLines_CommitsAndReturns201()
    {
        var vm = new GoodsReceiptVm
        {
            Header = new GoodsReceiptHeaderVm { SupplierId = 1, GrDate = DateTime.Now }
        };

        _grRepoMock
            .Setup(x => x.GrInsert(It.IsAny<GoodsReceiptHeader>(), _ct))
            .ReturnsAsync(1);

        var result = await _sut.GrInsert(vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(1, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GrInsert_SetsCreatedByFromUserClaim()
    {
        var vm = new GoodsReceiptVm
        {
            Header = new GoodsReceiptHeaderVm { SupplierId = 1, GrDate = DateTime.Now }
        };
        GoodsReceiptHeader? captured = null;

        _grRepoMock
            .Setup(x => x.GrInsert(It.IsAny<GoodsReceiptHeader>(), _ct))
            .Callback<GoodsReceiptHeader, CancellationToken>((h, _) => captured = h)
            .ReturnsAsync(1);

        await _sut.GrInsert(vm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task GrUpdate_WithValidGr_CommitsTransaction()
    {
        var vm = new GoodsReceiptHeaderVm { RecId = 1, GrId = "GR001", SupplierId = 1 };

        _grRepoMock
            .Setup(x => x.GrUpdate(It.IsAny<GoodsReceiptHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.GrUpdate(vm, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GrUpdate_SetsModifiedByFromUserClaim()
    {
        var vm = new GoodsReceiptHeaderVm { RecId = 1, GrId = "GR001", SupplierId = 1 };
        GoodsReceiptHeader? captured = null;

        _grRepoMock
            .Setup(x => x.GrUpdate(It.IsAny<GoodsReceiptHeader>(), _ct))
            .Callback<GoodsReceiptHeader, CancellationToken>((h, _) => captured = h)
            .Returns(Task.CompletedTask);

        await _sut.GrUpdate(vm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.ModifiedBy);
    }

    [Fact]
    public async Task GrDelete_WithValidId_CommitsTransaction()
    {
        var recId = 1;

        _grRepoMock
            .Setup(x => x.GrDelete(recId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.GrDelete(recId, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GrLineSelectByGr_WithValidId_ReturnsLines()
    {
        var grRecId = 1;
        var lines = new List<GoodsReceiptLine>
        {
            new() { GrLineId = 1, GrRecId = grRecId, ItemId = 10 },
            new() { GrLineId = 2, GrRecId = grRecId, ItemId = 20 }
        };

        _grRepoMock
            .Setup(x => x.GrLineSelectByGr(grRecId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.GrLineSelectByGr(grRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GrLineSelectById_WithValidId_ReturnsLine()
    {
        var grLineId = 1;
        var line = new GoodsReceiptLine { GrLineId = grLineId, GrRecId = 1, ItemId = 10 };

        _grRepoMock
            .Setup(x => x.GrLineSelectById(grLineId, _ct))
            .ReturnsAsync(line);

        var result = await _sut.GrLineSelectById(grLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(grLineId, result.Data.GrLineId);
    }

    [Fact]
    public async Task GrLineSelectById_WithInvalidId_ReturnsNotFound()
    {
        var grLineId = 999;

        _grRepoMock
            .Setup(x => x.GrLineSelectById(grLineId, _ct))
            .ReturnsAsync((GoodsReceiptLine?)null);

        var result = await _sut.GrLineSelectById(grLineId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GrLineInsert_WithValidLine_CommitsAndCreated()
    {
        var vm = new GoodsReceiptLineVm { GrRecId = 1, PoLineId = 5, Qty = 10, ItemId = 10 };

        _grRepoMock
            .Setup(x => x.GrLineInsert(It.IsAny<GoodsReceiptLine>(), _ct))
            .ReturnsAsync(5);

        var result = await _sut.GrLineInsert(vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GrLineInsert_SetsCreatedByFromUserClaim()
    {
        var vm = new GoodsReceiptLineVm { GrRecId = 1, PoLineId = 5, Qty = 10, ItemId = 10 };
        GoodsReceiptLine? captured = null;

        _grRepoMock
            .Setup(x => x.GrLineInsert(It.IsAny<GoodsReceiptLine>(), _ct))
            .Callback<GoodsReceiptLine, CancellationToken>((l, _) => captured = l)
            .ReturnsAsync(5);

        await _sut.GrLineInsert(vm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task GrLineUpdate_WithValidLine_CommitsTransaction()
    {
        var vm = new GoodsReceiptLineVm { GrLineId = 1, GrRecId = 1, PoLineId = 5, Qty = 20 };

        _grRepoMock
            .Setup(x => x.GrLineUpdate(It.IsAny<GoodsReceiptLine>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.GrLineUpdate(vm, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GrLineDelete_WithValidId_CommitsTransaction()
    {
        var grLineId = 1;

        _grRepoMock
            .Setup(x => x.GrLineDelete(grLineId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.GrLineDelete(grLineId, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoOsSelectBySupplier_WithValidSupplierId_ReturnsLines()
    {
        var supplierId = 1;
        var lines = new List<PoLineOutstanding>
        {
            new() { PoLineId = 1, QtyOs = 10 },
            new() { PoLineId = 2, QtyOs = 5 }
        };

        _grRepoMock
            .Setup(x => x.PoOsSelectBySupplier(supplierId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.PoOsSelectBySupplier(supplierId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetGoodsReceipt_WithValidId_ReturnsCompositeVm()
    {
        const int recId = 1;
        var header = new GoodsReceiptHeader { RecId = recId, GrId = "GR001", SupplierId = 1 };
        var lines = new List<GoodsReceiptLine>
        {
            new() { GrLineId = 1, GrRecId = recId, ItemId = 10 }
        };

        _grRepoMock
            .Setup(x => x.GrSelectById(recId, _ct))
            .ReturnsAsync(header);

        _grRepoMock
            .Setup(x => x.GrLineSelectByGr(recId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.GetGoodsReceipt(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Header);
        Assert.NotNull(result.Data.Lines);
        Assert.Equal("GR001", result.Data.Header.GrId);
        Assert.Single(result.Data.Lines);
    }

    [Fact]
    public async Task PoOsSelectById_WithExistingLine_ReturnsSingleItem()
    {
        var poLineId = 5;
        var line = new PoLineOutstanding { PoLineId = poLineId, PoId = "PO001", QtyOs = 10 };

        _grRepoMock
            .Setup(x => x.PoOsSelectById(poLineId, _ct))
            .ReturnsAsync(line);

        var result = await _sut.PoOsSelectById(poLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task PoOsSelectById_WithMissingLine_ReturnsEmptyArray()
    {
        _grRepoMock
            .Setup(x => x.PoOsSelectById(It.IsAny<int>(), _ct))
            .ReturnsAsync((PoLineOutstanding?)null);

        var result = await _sut.PoOsSelectById(999, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetGrReportData_WithDateRange_ReturnsData()
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 3, 31);
        var reportItems = new List<GrReportItem>
        {
            new() { GrId = "GR001" },
            new() { GrId = "GR002" }
        };

        _grRepoMock
            .Setup(x => x.GrRpt(startDate, endDate, _ct))
            .ReturnsAsync(reportItems);

        var result = await _sut.GetGrReportData(startDate, endDate, _ct);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetGrExportData_ReturnsHeaders()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 100 };
        var headers = new List<GoodsReceiptHeader>
        {
            new() { RecId = 1, GrId = "GR001" },
            new() { RecId = 2, GrId = "GR002" }
        };

        _grRepoMock
            .Setup(x => x.GrSelectForExport(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(headers);

        var result = await _sut.GetGrExportData(request, _ct);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
