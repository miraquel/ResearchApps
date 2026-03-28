namespace ResearchApps.Service.Tests;

public class CustomerOrderServiceTests
{
    private readonly Mock<ICustomerOrderRepo> _coRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly CustomerOrderService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public CustomerOrderServiceTests()
    {
        _coRepoMock = new Mock<ICustomerOrderRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<CustomerOrderService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new CustomerOrderService(
            _coRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task CoSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cos = new PagedList<CustomerOrderHeader>(
            new List<CustomerOrderHeader>
            {
                new() { RecId = 1, CoId = "CO001" },
                new() { RecId = 2, CoId = "CO002" }
            },
            1,
            10,
            2
        );

        _coRepoMock
            .Setup(x => x.CoSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(cos);

        var result = await _sut.CoSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Orders retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoSelectById_WithValidId_ReturnsCo()
    {
        var recId = 1;
        var co = new CustomerOrderHeader { RecId = recId, CoId = "CO001", CustomerId = 1 };

        _coRepoMock
            .Setup(x => x.CoSelectById(recId, _ct))
            .ReturnsAsync(co);

        var result = await _sut.CoSelectById(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoInsert_WithValidCo_ReturnsInsertedIdAndCoId()
    {
        var coVm = new CustomerOrderVm
        {
            Header = new CustomerOrderHeaderVm { CustomerId = 1, CoDate = DateTime.Now }
        };
        var insertResult = (RecId: 10, CoId: "CO010");

        _coRepoMock
            .Setup(x => x.CoInsert(It.IsAny<CustomerOrderHeader>(), _ct))
            .ReturnsAsync(insertResult);

        var result = await _sut.CoInsert(coVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoInsert_SetsCreatedByFromUserClaim()
    {
        var coVm = new CustomerOrderVm
        {
            Header = new CustomerOrderHeaderVm { CustomerId = 1 }
        };
        CustomerOrderHeader? capturedCo = null;

        _coRepoMock
            .Setup(x => x.CoInsert(It.IsAny<CustomerOrderHeader>(), _ct))
            .Callback<CustomerOrderHeader, CancellationToken>((c, _) => capturedCo = c)
            .ReturnsAsync((1, "CO001"));

        await _sut.CoInsert(coVm, _ct);

        Assert.NotNull(capturedCo);
        Assert.Equal(_userClaimDto.Username, capturedCo.CreatedBy);
    }

    [Fact]
    public async Task CoUpdate_WithValidCo_CommitsTransaction()
    {
        var coVm = new CustomerOrderHeaderVm { RecId = 1, CoId = "CO001", CustomerId = 1 };

        _coRepoMock
            .Setup(x => x.CoUpdate(It.IsAny<CustomerOrderHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoUpdate(coVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoDelete_WithValidId_CommitsTransaction()
    {
        var recId = 1;

        _coRepoMock
            .Setup(x => x.CoDelete(recId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoDelete(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoSubmitById_WithValidId_CommitsTransaction()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1 };

        _coRepoMock
            .Setup(x => x.CoSubmitById(action.RecId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoSubmitById(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order submitted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoRecallById_WithValidId_CommitsTransaction()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1 };

        _coRepoMock
            .Setup(x => x.CoRecallById(action.RecId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoRecallById(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order recalled successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoRejectById_WithValidAction_CommitsTransaction()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1, Notes = "Rejected" };

        _coRepoMock
            .Setup(x => x.CoRejectById(action.RecId, _userClaimDto.Username, action.Notes, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoRejectById(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order rejected successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoCloseByNo_WithValidCoId_CommitsTransaction()
    {
        var action = new CustomerOrderWorkflowActionVm { CoId = "CO001" };

        _coRepoMock
            .Setup(x => x.CoCloseByNo(action.CoId!, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoCloseByNo(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order closed successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoLineSelectByCo_WithValidCoRecId_ReturnsCoLines()
    {
        var coRecId = 1;
        var coLines = new List<CustomerOrderLine>
        {
            new() { CoLineId = 1, CoRecId = coRecId, ItemId = 1 },
            new() { CoLineId = 2, CoRecId = coRecId, ItemId = 2 }
        };

        _coRepoMock
            .Setup(x => x.CoLineSelectByCo(coRecId, _ct))
            .ReturnsAsync(coLines);

        var result = await _sut.CoLineSelectByCo(coRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("CO lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoLineInsert_WithValidCoLine_ReturnsInsertedId()
    {
        var coLineVm = new CustomerOrderLineVm { CoRecId = 1, ItemId = 1, Qty = 10 };
        var insertedLineNo = "5";

        _coRepoMock
            .Setup(x => x.CoLineInsert(It.IsAny<CustomerOrderLine>(), _ct))
            .ReturnsAsync(insertedLineNo);

        var result = await _sut.CoLineInsert(coLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("CO line inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoLineUpdate_WithValidCoLine_CommitsTransaction()
    {
        var coLineVm = new CustomerOrderLineVm { CoLineId = 1, CoRecId = 1, ItemId = 1, Qty = 20 };

        _coRepoMock
            .Setup(x => x.CoLineUpdate(It.IsAny<CustomerOrderLine>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoLineUpdate(coLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("CO line updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoHdOsSelect_WithValidCustomerId_ReturnsOutstandingHeaders()
    {
        var customerId = 1;
        var headers = new List<CustomerOrderHeaderOutstanding>
        {
            new() { CoRecId = 1, CoId = "CO001", CustomerId = customerId }
        };

        _coRepoMock
            .Setup(x => x.CoHdOsSelect(customerId, _ct))
            .ReturnsAsync(headers);

        var result = await _sut.CoHdOsSelect(customerId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding CO headers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoOsSelect_WithValidCustomerId_ReturnsOutstandingLines()
    {
        var customerId = 1;
        var lines = new List<CustomerOrderLineOutstanding>
        {
            new() { CoLineId = 1, CoId = "CO001", ItemId = 1, QtyCo = 10 }
        };

        _coRepoMock
            .Setup(x => x.CoOsSelect(customerId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.CoOsSelect(customerId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding CO lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var coVm = new CustomerOrderVm
        {
            Header = new CustomerOrderHeaderVm { CustomerId = 1 }
        };

        _coRepoMock
            .Setup(x => x.CoInsert(It.IsAny<CustomerOrderHeader>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CoInsert(coVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CoApproveById_WithValidAction_CommitsTransaction()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1, Notes = "Approved" };

        _coRepoMock
            .Setup(x => x.CoApproveById(action.RecId, _userClaimDto.Username, action.Notes, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CoApproveById(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order approved successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoApproveById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1, Notes = "Approved" };

        _coRepoMock
            .Setup(x => x.CoApproveById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Approval not allowed"));

        var result = await _sut.CoApproveById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CoSubmitById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1 };

        _coRepoMock
            .Setup(x => x.CoSubmitById(It.IsAny<int>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("No workflow configured"));

        var result = await _sut.CoSubmitById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CoRecallById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1 };

        _coRepoMock
            .Setup(x => x.CoRecallById(It.IsAny<int>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Cannot recall at this stage"));

        var result = await _sut.CoRecallById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CoRejectById_WhenRepoThrowsRepoException_ReturnsFailure()
    {
        var action = new CustomerOrderWorkflowActionVm { RecId = 1, Notes = "Rejected" };

        _coRepoMock
            .Setup(x => x.CoRejectById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException("Cannot reject at this stage"));

        var result = await _sut.CoRejectById(action, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CoCloseByNo_WithNullCoId_ReturnsFailure()
    {
        var action = new CustomerOrderWorkflowActionVm { CoId = null };

        var result = await _sut.CoCloseByNo(action, _ct);

        Assert.False(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CoLineSelectById_WithExistingId_ReturnsLine()
    {
        var coLineId = 5;
        var line = new CustomerOrderLine { CoLineId = coLineId, CoRecId = 1, ItemId = 10 };

        _coRepoMock
            .Setup(x => x.CoLineSelectById(coLineId, _ct))
            .ReturnsAsync(line);

        var result = await _sut.CoLineSelectById(coLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("CO line retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoLineSelectById_WithMissingId_ReturnsNotFound()
    {
        _coRepoMock
            .Setup(x => x.CoLineSelectById(It.IsAny<int>(), _ct))
            .ReturnsAsync((CustomerOrderLine?)null);

        var result = await _sut.CoLineSelectById(999, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task CoOsById_WithLinesPresent_ReturnsSuccess()
    {
        var coRecId = 1;
        var lines = new List<CustomerOrderLineOutstanding>
        {
            new() { CoLineId = 1, CoId = "CO001", ItemId = 10, QtyCo = 5 }
        };

        _coRepoMock
            .Setup(x => x.CoOsById(coRecId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.CoOsById(coRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding CO lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoOsById_WithNoLines_ReturnsNotFound()
    {
        _coRepoMock
            .Setup(x => x.CoOsById(It.IsAny<int>(), _ct))
            .ReturnsAsync(new List<CustomerOrderLineOutstanding>());

        var result = await _sut.CoOsById(99, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task CoOsByCoLineId_WithExistingLine_ReturnsSuccess()
    {
        var coLineId = 3;
        var line = new CustomerOrderLineOutstanding { CoLineId = coLineId, CoId = "CO001", ItemId = 10 };

        _coRepoMock
            .Setup(x => x.CoOsByCoLineId(coLineId, _ct))
            .ReturnsAsync(line);

        var result = await _sut.CoOsByCoLineId(coLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding CO line retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoOsByCoLineId_WithMissingLine_ReturnsNotFound()
    {
        _coRepoMock
            .Setup(x => x.CoOsByCoLineId(It.IsAny<int>(), _ct))
            .ReturnsAsync((CustomerOrderLineOutstanding?)null);

        var result = await _sut.CoOsByCoLineId(999, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task CoLineSelectByCo_WithNoLines_ReturnsNotFound()
    {
        _coRepoMock
            .Setup(x => x.CoLineSelectByCo(It.IsAny<int>(), _ct))
            .ReturnsAsync(new List<CustomerOrderLine>());

        var result = await _sut.CoLineSelectByCo(99, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task CoTypeCbo_ReturnsCoTypes()
    {
        var types = new List<CustomerOrderType>
        {
            new() { CoTypeId = 1, CoTypeName = "Standard" },
            new() { CoTypeId = 2, CoTypeName = "Rush" }
        };

        _coRepoMock
            .Setup(x => x.CoTypeCbo(_ct))
            .ReturnsAsync(types);

        var result = await _sut.CoTypeCbo(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("CO types retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task GetCustomerOrder_WithValidId_ReturnsCompositeVm()
    {
        var recId = 1;
        var header = new CustomerOrderHeader { RecId = recId, CoId = "CO001", CustomerId = 5 };
        var lines = new List<CustomerOrderLine> { new() { CoLineId = 1, CoRecId = recId, ItemId = 10 } };
        var outstanding = new List<CustomerOrderLineOutstanding> { new() { CoLineId = 2, CoId = "CO001", ItemId = 20 } };

        _coRepoMock.Setup(x => x.CoSelectById(recId, _ct)).ReturnsAsync(header);
        _coRepoMock.Setup(x => x.CoLineSelectByCo(recId, _ct)).ReturnsAsync(lines);
        _coRepoMock.Setup(x => x.CoOsSelect(header.CustomerId, _ct)).ReturnsAsync(outstanding);

        var result = await _sut.GetCustomerOrder(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order ViewModel retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetCoSummaryReportData_WithDateRange_ReturnsData()
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 3, 31);
        var reportItems = new List<CoSummaryReportItem>
        {
            new() { No = 1, CustomerName = "Customer A", Amount = 1000 },
            new() { No = 2, CustomerName = "Customer B", Amount = 2000 }
        };

        _coRepoMock
            .Setup(x => x.CoRptSummary(startDate, endDate, _ct))
            .ReturnsAsync(reportItems);

        var result = await _sut.GetCoSummaryReportData(startDate, endDate, _ct);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCoDetailReportData_WithDateRange_ReturnsData()
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 3, 31);
        var reportItems = new List<CoDetailReportItem> { new() };

        _coRepoMock
            .Setup(x => x.CoRptDetail(startDate, endDate, _ct))
            .ReturnsAsync(reportItems);

        var result = await _sut.GetCoDetailReportData(startDate, endDate, _ct);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetCoExportData_ReturnsHeaders()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 100 };
        var headers = new List<CustomerOrderHeader>
        {
            new() { RecId = 1, CoId = "CO001" },
            new() { RecId = 2, CoId = "CO002" }
        };

        _coRepoMock
            .Setup(x => x.CoSelectForExport(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(headers);

        var result = await _sut.GetCoExportData(request, _ct);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetWfHistory_WithValidRefId_ReturnsHistory()
    {
        var refId = "CO001";
        var wfFormId = 1;
        var historyItems = new List<WfTransHistory>
        {
            new() { WfTransId = 1, RefId = refId, ActionDate = DateTime.Now, Notes = "Submitted" },
            new() { WfTransId = 2, RefId = refId, ActionDate = DateTime.Now, Notes = "Approved" }
        };

        _coRepoMock
            .Setup(x => x.WfTransSelectByRefId(refId, wfFormId, _ct))
            .ReturnsAsync(historyItems);

        var result = await _sut.GetWfHistory(refId, wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow history retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }
}
