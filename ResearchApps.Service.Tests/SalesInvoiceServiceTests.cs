namespace ResearchApps.Service.Tests;

public class SalesInvoiceServiceTests
{
    private readonly Mock<ISalesInvoiceRepo> _siRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly SalesInvoiceService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public SalesInvoiceServiceTests()
    {
        _siRepoMock = new Mock<ISalesInvoiceRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<SalesInvoiceService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new SalesInvoiceService(
            _siRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SiSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var sis = new PagedList<SalesInvoiceHeader>(
            new List<SalesInvoiceHeader>
            {
                new() { RecId = 1, SiId = "FNAINV24001" },
                new() { RecId = 2, SiId = "FNAINV24002" }
            },
            1,
            10,
            2
        );

        _siRepoMock
            .Setup(x => x.SiSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(sis);

        var result = await _sut.SiSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoices retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalCount);
    }

    [Fact]
    public async Task SiSelectById_WithValidId_ReturnsSi()
    {
        var recId = 1;
        var si = new SalesInvoiceHeader { RecId = recId, SiId = "FNAINV24001", CustomerId = 1 };

        _siRepoMock
            .Setup(x => x.SiSelectById(recId, _ct))
            .ReturnsAsync(si);

        var result = await _sut.SiSelectById(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("FNAINV24001", result.Data.SiId);
    }

    [Fact]
    public async Task SiInsert_WithValidSi_ReturnsInsertedIdAndSiId()
    {
        var siVm = new SalesInvoiceVm
        {
            Header = new SalesInvoiceHeaderVm { CustomerId = 1, SiDate = DateTime.Now },
            Lines = new List<SalesInvoiceLineVm>
            {
                new() { DoLineId = 1, DoId = "DO001", ItemId = 1, Qty = 10, Price = 1000 }
            }
        };
        var insertResult = (RecId: 10, SiId: "FNAINV24010");

        _siRepoMock
            .Setup(x => x.SiInsert(It.IsAny<SalesInvoiceHeader>(), _ct))
            .ReturnsAsync(insertResult);

        _siRepoMock
            .Setup(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), _ct))
            .ReturnsAsync("1:::FNAINV24010");

        var result = await _sut.SiInsert(siVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SiInsert_SetsCreatedByFromUserClaim()
    {
        var siVm = new SalesInvoiceVm
        {
            Header = new SalesInvoiceHeaderVm { CustomerId = 1 }
        };
        SalesInvoiceHeader? capturedSi = null;

        _siRepoMock
            .Setup(x => x.SiInsert(It.IsAny<SalesInvoiceHeader>(), _ct))
            .Callback<SalesInvoiceHeader, CancellationToken>((s, _) => capturedSi = s)
            .ReturnsAsync((1, "FNAINV24001"));

        await _sut.SiInsert(siVm, _ct);

        Assert.NotNull(capturedSi);
        Assert.Equal(_userClaimDto.Username, capturedSi.CreatedBy);
    }

    [Fact]
    public async Task SiInsert_WithLines_InsertsAllLines()
    {
        var siVm = new SalesInvoiceVm
        {
            Header = new SalesInvoiceHeaderVm { CustomerId = 1, SiDate = DateTime.Now },
            Lines = new List<SalesInvoiceLineVm>
            {
                new() { DoLineId = 1, DoId = "DO001", ItemId = 1, Qty = 10, Price = 1000 },
                new() { DoLineId = 2, DoId = "DO001", ItemId = 2, Qty = 5, Price = 2000 },
                new() { DoLineId = 3, DoId = "DO002", ItemId = 3, Qty = 3, Price = 3000 }
            }
        };

        _siRepoMock
            .Setup(x => x.SiInsert(It.IsAny<SalesInvoiceHeader>(), _ct))
            .ReturnsAsync((1, "FNAINV24001"));

        _siRepoMock
            .Setup(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), _ct))
            .ReturnsAsync("1:::FNAINV24001");

        await _sut.SiInsert(siVm, _ct);

        _siRepoMock.Verify(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), _ct), Times.Exactly(3));
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SiUpdate_WithValidSi_CommitsTransaction()
    {
        var siVm = new SalesInvoiceHeaderVm { RecId = 1, SiId = "FNAINV24001", CustomerId = 1 };

        _siRepoMock
            .Setup(x => x.SiUpdate(It.IsAny<SalesInvoiceHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.SiUpdate(siVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SiUpdate_SetsModifiedByFromUserClaim()
    {
        var siVm = new SalesInvoiceHeaderVm { RecId = 1, SiId = "FNAINV24001", CustomerId = 1 };
        SalesInvoiceHeader? capturedSi = null;

        _siRepoMock
            .Setup(x => x.SiUpdate(It.IsAny<SalesInvoiceHeader>(), _ct))
            .Callback<SalesInvoiceHeader, CancellationToken>((s, _) => capturedSi = s)
            .Returns(Task.CompletedTask);

        await _sut.SiUpdate(siVm, _ct);

        Assert.NotNull(capturedSi);
        Assert.Equal(_userClaimDto.Username, capturedSi.ModifiedBy);
    }

    [Fact]
    public async Task SiDelete_WithValidId_CommitsTransaction()
    {
        var recId = 1;

        _siRepoMock
            .Setup(x => x.SiDelete(recId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.SiDelete(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SiLineSelectBySi_WithValidSiRecId_ReturnsLines()
    {
        var siRecId = 1;
        var lines = new List<SalesInvoiceLine>
        {
            new() { SiLineId = 1, SiId = "FNAINV24001", ItemId = 1, Qty = 10, Price = 1000 },
            new() { SiLineId = 2, SiId = "FNAINV24001", ItemId = 2, Qty = 5, Price = 2000 }
        };

        _siRepoMock
            .Setup(x => x.SiLineSelectBySi(siRecId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.SiLineSelectBySi(siRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice lines retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task SiLineSelectById_WithValidId_ReturnsLine()
    {
        var siLineId = 1;
        var line = new SalesInvoiceLine { SiLineId = siLineId, SiId = "FNAINV24001", ItemId = 1 };

        _siRepoMock
            .Setup(x => x.SiLineSelectById(siLineId, _ct))
            .ReturnsAsync(line);

        var result = await _sut.SiLineSelectById(siLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice line retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task SiLineSelectById_WithInvalidId_ReturnsFailure()
    {
        var siLineId = 999;

        _siRepoMock
            .Setup(x => x.SiLineSelectById(siLineId, _ct))
            .ReturnsAsync((SalesInvoiceLine?)null);

        var result = await _sut.SiLineSelectById(siLineId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("not found"));
    }

    [Fact]
    public async Task SiLineInsert_WithValidLine_CommitsTransaction()
    {
        var lineVm = new SalesInvoiceLineVm
        {
            SiRecId = 1,
            DoLineId = 1,
            DoId = "DO001",
            ItemId = 1,
            Qty = 10,
            Price = 1000
        };

        _siRepoMock
            .Setup(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), _ct))
            .ReturnsAsync("1:::FNAINV24001");

        var result = await _sut.SiLineInsert(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice line inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetSalesInvoice_WithValidId_ReturnsHeaderAndLines()
    {
        var recId = 1;
        var header = new SalesInvoiceHeader
        {
            RecId = recId,
            SiId = "FNAINV24001",
            CustomerId = 1,
            CustomerName = "Test Customer",
            Amount = 15000
        };
        var lines = new List<SalesInvoiceLine>
        {
            new() { SiLineId = 1, SiId = "FNAINV24001", ItemId = 1, Qty = 10, Price = 1000 },
            new() { SiLineId = 2, SiId = "FNAINV24001", ItemId = 2, Qty = 5, Price = 1000 }
        };

        _siRepoMock
            .Setup(x => x.SiSelectById(recId, _ct))
            .ReturnsAsync(header);

        _siRepoMock
            .Setup(x => x.SiLineSelectBySi(recId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.GetSalesInvoice(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("FNAINV24001", result.Data.Header.SiId);
        Assert.Equal(2, result.Data.Lines.Count());
    }
}
