namespace ResearchApps.Service.Tests;

public class DeliveryOrderServiceTests
{
    private readonly Mock<IDeliveryOrderRepo> _doRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly Mock<IItemRepo> _itemRepoMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly DeliveryOrderService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public DeliveryOrderServiceTests()
    {
        _doRepoMock = new Mock<IDeliveryOrderRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _itemRepoMock = new Mock<IItemRepo>();
        var loggerMock = new Mock<ILogger<DeliveryOrderService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new DeliveryOrderService(
            _doRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object,
            _itemRepoMock.Object);
    }

    [Fact]
    public async Task DoSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var dos = new PagedList<DeliveryOrderHeader>(
            new List<DeliveryOrderHeader>
            {
                new() { RecId = 1, DoId = "DO001" },
                new() { RecId = 2, DoId = "DO002" }
            },
            1,
            10,
            2
        );

        _doRepoMock
            .Setup(x => x.DoSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(dos);

        var result = await _sut.DoSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Orders retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoSelectById_WithValidId_ReturnsDo()
    {
        var recId = 1;
        var deliveryOrder = new DeliveryOrderHeader { RecId = recId, DoId = "DO001", CustomerId = 1 };

        _doRepoMock
            .Setup(x => x.DoSelectById(recId, _ct))
            .ReturnsAsync(deliveryOrder);

        var result = await _sut.DoSelectById(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoInsert_WithValidDo_ReturnsInsertedIdAndDoId()
    {
        var doVm = new DeliveryOrderVm
        {
            Header = new DeliveryOrderHeaderVm { CustomerId = 1, DoDate = DateTime.Now }
        };
        var insertResult = (RecId: 10, DoId: "DO010");

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), _ct))
            .ReturnsAsync(insertResult);

        var result = await _sut.DoInsert(doVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoInsert_SetsCreatedByFromUserClaim()
    {
        var doVm = new DeliveryOrderVm
        {
            Header = new DeliveryOrderHeaderVm { CustomerId = 1 }
        };
        DeliveryOrderHeader? capturedDo = null;

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), _ct))
            .Callback<DeliveryOrderHeader, CancellationToken>((d, _) => capturedDo = d)
            .ReturnsAsync((1, "DO001"));

        await _sut.DoInsert(doVm, _ct);

        Assert.NotNull(capturedDo);
        Assert.Equal(_userClaimDto.Username, capturedDo.CreatedBy);
    }

    [Fact]
    public async Task DoUpdate_WithValidDo_CommitsTransaction()
    {
        var doVm = new DeliveryOrderHeaderVm { RecId = 1, DoId = "DO001", CustomerId = 1 };

        _doRepoMock
            .Setup(x => x.DoUpdate(It.IsAny<DeliveryOrderHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DoUpdate(doVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoUpdate_SetsModifiedByFromUserClaim()
    {
        var doVm = new DeliveryOrderHeaderVm { RecId = 1, DoId = "DO001", CustomerId = 1 };
        DeliveryOrderHeader? capturedDo = null;

        _doRepoMock
            .Setup(x => x.DoUpdate(It.IsAny<DeliveryOrderHeader>(), _ct))
            .Callback<DeliveryOrderHeader, CancellationToken>((d, _) => capturedDo = d)
            .Returns(Task.CompletedTask);

        await _sut.DoUpdate(doVm, _ct);

        Assert.NotNull(capturedDo);
        Assert.Equal(_userClaimDto.Username, capturedDo.ModifiedBy);
    }

    [Fact]
    public async Task DoDelete_WithValidId_CommitsTransaction()
    {
        var recId = 1;

        _doRepoMock
            .Setup(x => x.DoDelete(recId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DoDelete(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoLineSelectByDo_WithValidDoRecId_ReturnsDoLines()
    {
        var doRecId = 1;
        var doLines = new List<DeliveryOrderLine>
        {
            new() { DoLineId = 1, DoRecId = doRecId, ItemId = 1 },
            new() { DoLineId = 2, DoRecId = doRecId, ItemId = 2 }
        };

        _doRepoMock
            .Setup(x => x.DoLineSelectByDo(doRecId, _ct))
            .ReturnsAsync(doLines);

        var result = await _sut.DoLineSelectByDo(doRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("DO lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoLineSelectById_WithValidId_ReturnsDoLine()
    {
        var doLineId = 1;
        var doLine = new DeliveryOrderLine { DoLineId = doLineId, DoRecId = 1, ItemId = 1 };

        _doRepoMock
            .Setup(x => x.DoLineSelectById(doLineId, _ct))
            .ReturnsAsync(doLine);

        var result = await _sut.DoLineSelectById(doLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("DO line retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoLineInsert_WithValidDoLine_ReturnsInsertedId()
    {
        var doLineVm = new DeliveryOrderLineVm { DoRecId = 1, ItemId = 1, Qty = 10 };
        var insertedLineNo = "5";

        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), _ct))
            .ReturnsAsync(insertedLineNo);

        var result = await _sut.DoLineInsert(doLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("DO line inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoLineInsert_SetsCreatedByFromUserClaim()
    {
        var doLineVm = new DeliveryOrderLineVm { DoRecId = 1, ItemId = 1, Qty = 10 };
        DeliveryOrderLine? capturedDoLine = null;

        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), _ct))
            .Callback<DeliveryOrderLine, CancellationToken>((dl, _) => capturedDoLine = dl)
            .ReturnsAsync("1");

        await _sut.DoLineInsert(doLineVm, _ct);

        Assert.NotNull(capturedDoLine);
        Assert.Equal(_userClaimDto.Username, capturedDoLine.CreatedBy);
    }

    [Fact]
    public async Task DoLineUpdate_WithValidDoLine_CommitsTransaction()
    {
        var doLineVm = new DeliveryOrderLineVm { DoLineId = 1, DoRecId = 1, ItemId = 1, Qty = 20 };

        _doRepoMock
            .Setup(x => x.DoLineUpdate(It.IsAny<DeliveryOrderLine>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DoLineUpdate(doLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("DO line updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoLineDelete_WithValidId_CommitsTransaction()
    {
        var doLineId = 1;

        _doRepoMock
            .Setup(x => x.DoLineDelete(doLineId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DoLineDelete(doLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("DO line deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoHdOsSelect_WithValidCustomerId_ReturnsOutstandingHeaders()
    {
        var customerId = 1;
        var headers = new List<DeliveryOrderHeaderOutstanding>
        {
            new() { DoRecId = 1, DoId = "DO001", CustomerId = customerId }
        };

        _doRepoMock
            .Setup(x => x.DoHdOsSelect(customerId, _ct))
            .ReturnsAsync(headers);

        var result = await _sut.DoHdOsSelect(customerId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding DO headers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoOsSelect_WithValidCustomerId_ReturnsOutstandingLines()
    {
        var customerId = 1;
        var lines = new List<DeliveryOrderLineOutstanding>
        {
            new() { DoLineId = 1, DoId = "DO001", ItemId = 1, QtyDo = 10 }
        };

        _doRepoMock
            .Setup(x => x.DoOsSelect(customerId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.DoOsSelect(customerId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding DO lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task GetDeliveryOrderViewModel_WithValidRecId_ReturnsCompleteViewModel()
    {
        const int recId = 1;
        var composite = new DeliveryOrder
        {
            Header = new DeliveryOrderHeader { RecId = recId, DoId = "DO001", CustomerId = 1 },
            Lines = new List<DeliveryOrderLine>
            {
                new() { DoLineId = 1, DoRecId = recId, ItemId = 1 }
            },
            Outstanding = new List<DeliveryOrderLineOutstanding>
            {
                new() { DoLineId = 2, DoId = "DO002", ItemId = 2 }
            }
        };

        _doRepoMock.Setup(x => x.DoSelectCompositeById(recId, _ct))
            .ReturnsAsync(composite);

        var result = await _sut.GetDeliveryOrderViewModel(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order ViewModel retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.NotNull(data.Header);
        Assert.NotNull(data.Lines);
        Assert.NotNull(data.Outstanding);
    }

    [Fact]
    public async Task DoInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var doVm = new DeliveryOrderVm
        {
            Header = new DeliveryOrderHeaderVm { CustomerId = 1 }
        };

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.DoInsert(doVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task DoInsert_WithLines_InsertsEachLineAndCommits()
    {
        var doVm = new DeliveryOrderVm
        {
            Header = new DeliveryOrderHeaderVm { CustomerId = 1, CoId = "CO001" },
            Lines =
            [
                new DeliveryOrderLineVm { ItemId = 10, Qty = 5, ItemName = "Item A" },
                new DeliveryOrderLineVm { ItemId = 20, Qty = 3 }
            ]
        };

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), _ct))
            .ReturnsAsync((RecId: 1, DoId: "DO001"));
        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), _ct))
            .ReturnsAsync("DO001-1");

        var result = await _sut.DoInsert(doVm, _ct);

        Assert.True(result.IsSuccess);
        _doRepoMock.Verify(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), _ct), Times.Exactly(2));
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoInsert_WithLines_WhenCoIdIsSet_KeepsExistingCoId()
    {
        var doVm = new DeliveryOrderVm
        {
            Header = new DeliveryOrderHeaderVm { CustomerId = 1, CoId = "CO-HEADER" },
            Lines =
            [
                new DeliveryOrderLineVm { ItemId = 10, Qty = 1, CoId = "CO-LINE-EXISTING" }
            ]
        };

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), _ct))
            .ReturnsAsync((RecId: 1, DoId: "DO001"));

        DeliveryOrderLine? captured = null;
        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), _ct))
            .Callback<DeliveryOrderLine, CancellationToken>((l, _) => captured = l)
            .ReturnsAsync("DO001-1");

        await _sut.DoInsert(doVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal("CO-LINE-EXISTING", captured.CoId);
    }

    [Fact]
    public async Task DoInsert_WhenDoLineInsertThrowsSqlException_WrapsInRepoException()
    {
        var doVm = new DeliveryOrderVm
        {
            Header = new DeliveryOrderHeaderVm { CustomerId = 1 },
            Lines = [new DeliveryOrderLineVm { ItemId = 10, Qty = 1, ItemName = "Widget" }]
        };
        var sqlEx = (SqlException)System.Runtime.CompilerServices.RuntimeHelpers
            .GetUninitializedObject(typeof(SqlException));

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), _ct))
            .ReturnsAsync((RecId: 1, DoId: "DO001"));
        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), _ct))
            .ThrowsAsync(sqlEx);

        var ex = await Assert.ThrowsAsync<RepoException>(async () =>
            await _sut.DoInsert(doVm, _ct));

        Assert.Contains("Widget", ex.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task DoInsert_WhenDoLineInsertThrowsSqlException_UsesItemIdWhenNameEmpty()
    {
        var doVm = new DeliveryOrderVm
        {
            Header = new DeliveryOrderHeaderVm { CustomerId = 1 },
            Lines = [new DeliveryOrderLineVm { ItemId = 42, Qty = 1, ItemName = null }]
        };
        var sqlEx = (SqlException)System.Runtime.CompilerServices.RuntimeHelpers
            .GetUninitializedObject(typeof(SqlException));

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), _ct))
            .ReturnsAsync((RecId: 1, DoId: "DO001"));
        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), _ct))
            .ThrowsAsync(sqlEx);

        var ex = await Assert.ThrowsAsync<RepoException>(async () =>
            await _sut.DoInsert(doVm, _ct));

        Assert.Contains("ItemId 42", ex.Message);
    }

    [Fact]
    public async Task DoOsByDoLineId_WithValidId_ReturnsOutstandingLine()
    {
        var doLineId = 3;
        var line = new DeliveryOrderLineOutstanding { DoLineId = doLineId, DoId = "DO001", ItemId = 10 };

        _doRepoMock
            .Setup(x => x.DoOsByDoLineId(doLineId, _ct))
            .ReturnsAsync(line);

        var result = await _sut.DoOsByDoLineId(doLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding DO line retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoLineDelete_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var doLineId = 1;

        _doRepoMock
            .Setup(x => x.DoLineDelete(doLineId, _userClaimDto.Username, _ct))
            .ThrowsAsync(new RepoException("Line cannot be deleted"));

        var result = await _sut.DoLineDelete(doLineId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetDoExportData_ReturnsHeaders()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 100 };
        var headers = new List<DeliveryOrderHeader>
        {
            new() { RecId = 1, DoId = "DO001" },
            new() { RecId = 2, DoId = "DO002" }
        };

        _doRepoMock
            .Setup(x => x.DoSelectForExport(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(headers);

        var result = await _sut.GetDoExportData(request, _ct);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetWfHistory_WithValidRefId_ReturnsHistory()
    {
        var refId = "DO001";
        var wfFormId = 4;
        var historyItems = new List<WfTransHistory>
        {
            new() { WfTransId = 1, RefId = refId, ActionDate = DateTime.Now, Notes = "Created" }
        };

        _doRepoMock
            .Setup(x => x.WfTransSelectByRefId(refId, wfFormId, _ct))
            .ReturnsAsync(historyItems);

        var result = await _sut.GetWfHistory(refId, wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow history retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }
}
