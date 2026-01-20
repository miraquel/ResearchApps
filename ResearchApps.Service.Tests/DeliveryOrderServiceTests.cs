namespace ResearchApps.Service.Tests;

/// <summary>
/// Unit tests for DoService covering Delivery Order operations
/// </summary>
public class DeliveryOrderServiceTests
{
    private readonly Mock<IDeliveryOrderRepo> _doRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly Mock<IItemRepo> _itemRepoMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly DeliveryOrderService _sut;

    public DeliveryOrderServiceTests()
    {
        _doRepoMock = new Mock<IDeliveryOrderRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _itemRepoMock = new Mock<IItemRepo>();
        var loggerMock = new Mock<ILogger<DeliveryOrderService>>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new DeliveryOrderService(
            _doRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object,
            _itemRepoMock.Object);
    }

    #region DO Header Tests

    [Fact]
    public async Task DoSelect_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
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
            .Setup(x => x.DoSelect(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(dos);

        // Act
        var result = await _sut.DoSelect(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Orders retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoSelectById_WithValidId_ReturnsDo()
    {
        // Arrange
        var recId = 1;
        var cancellationToken = CancellationToken.None;
        var deliveryOrder = new DeliveryOrderHeader { RecId = recId, DoId = "DO001", CustomerId = 1 };

        _doRepoMock
            .Setup(x => x.DoSelectById(recId, cancellationToken))
            .ReturnsAsync(deliveryOrder);

        // Act
        var result = await _sut.DoSelectById(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoInsert_WithValidDo_ReturnsInsertedIdAndDoId()
    {
        // Arrange
        var doVm = new DeliveryOrderVm 
        { 
            Header = new DeliveryOrderHeaderVm { CustomerId = 1, DoDate = DateTime.Now } 
        };
        var cancellationToken = CancellationToken.None;
        var insertResult = (RecId: 10, DoId: "DO010");

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), cancellationToken))
            .ReturnsAsync(insertResult);

        // Act
        var result = await _sut.DoInsert(doVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoInsert_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var doVm = new DeliveryOrderVm 
        { 
            Header = new DeliveryOrderHeaderVm { CustomerId = 1 } 
        };
        var cancellationToken = CancellationToken.None;
        DeliveryOrderHeader? capturedDo = null;

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), cancellationToken))
            .Callback<DeliveryOrderHeader, CancellationToken>((d, _) => capturedDo = d)
            .ReturnsAsync((1, "DO001"));

        // Act
        await _sut.DoInsert(doVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedDo);
        Assert.Equal(_userClaimDto.Username, capturedDo.CreatedBy);
    }

    [Fact]
    public async Task DoUpdate_WithValidDo_CommitsTransaction()
    {
        // Arrange
        var doVm = new DeliveryOrderHeaderVm { RecId = 1, DoId = "DO001", CustomerId = 1 };
        var cancellationToken = CancellationToken.None;

        _doRepoMock
            .Setup(x => x.DoUpdate(It.IsAny<DeliveryOrderHeader>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DoUpdate(doVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoUpdate_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var doVm = new DeliveryOrderHeaderVm { RecId = 1, DoId = "DO001", CustomerId = 1 };
        var cancellationToken = CancellationToken.None;
        DeliveryOrderHeader? capturedDo = null;

        _doRepoMock
            .Setup(x => x.DoUpdate(It.IsAny<DeliveryOrderHeader>(), cancellationToken))
            .Callback<DeliveryOrderHeader, CancellationToken>((d, _) => capturedDo = d)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DoUpdate(doVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedDo);
        Assert.Equal(_userClaimDto.Username, capturedDo.ModifiedBy);
    }

    [Fact]
    public async Task DoDelete_WithValidId_CommitsTransaction()
    {
        // Arrange
        var recId = 1;
        var cancellationToken = CancellationToken.None;

        _doRepoMock
            .Setup(x => x.DoDelete(recId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DoDelete(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region DO Line Tests

    [Fact]
    public async Task DoLineSelectByDo_WithValidDoRecId_ReturnsDoLines()
    {
        // Arrange
        var doRecId = 1;
        var cancellationToken = CancellationToken.None;
        var doLines = new List<DeliveryOrderLine>
        {
            new() { DoLineId = 1, DoRecId = doRecId, ItemId = 1 },
            new() { DoLineId = 2, DoRecId = doRecId, ItemId = 2 }
        };

        _doRepoMock
            .Setup(x => x.DoLineSelectByDo(doRecId, cancellationToken))
            .ReturnsAsync(doLines);

        // Act
        var result = await _sut.DoLineSelectByDo(doRecId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("DO lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoLineSelectById_WithValidId_ReturnsDoLine()
    {
        // Arrange
        var doLineId = 1;
        var cancellationToken = CancellationToken.None;
        var doLine = new DeliveryOrderLine { DoLineId = doLineId, DoRecId = 1, ItemId = 1 };

        _doRepoMock
            .Setup(x => x.DoLineSelectById(doLineId, cancellationToken))
            .ReturnsAsync(doLine);

        // Act
        var result = await _sut.DoLineSelectById(doLineId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("DO line retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoLineInsert_WithValidDoLine_ReturnsInsertedId()
    {
        // Arrange
        var doLineVm = new DeliveryOrderLineVm { DoRecId = 1, ItemId = 1, Qty = 10 };
        var cancellationToken = CancellationToken.None;
        var insertedLineNo = "5";

        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), cancellationToken))
            .ReturnsAsync(insertedLineNo);

        // Act
        var result = await _sut.DoLineInsert(doLineVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("DO line inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoLineInsert_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var doLineVm = new DeliveryOrderLineVm { DoRecId = 1, ItemId = 1, Qty = 10 };
        var cancellationToken = CancellationToken.None;
        DeliveryOrderLine? capturedDoLine = null;

        _doRepoMock
            .Setup(x => x.DoLineInsert(It.IsAny<DeliveryOrderLine>(), cancellationToken))
            .Callback<DeliveryOrderLine, CancellationToken>((dl, _) => capturedDoLine = dl)
            .ReturnsAsync("1");

        // Act
        await _sut.DoLineInsert(doLineVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedDoLine);
        Assert.Equal(_userClaimDto.Username, capturedDoLine.CreatedBy);
    }

    [Fact]
    public async Task DoLineUpdate_WithValidDoLine_CommitsTransaction()
    {
        // Arrange
        var doLineVm = new DeliveryOrderLineVm { DoLineId = 1, DoRecId = 1, ItemId = 1, Qty = 20 };
        var cancellationToken = CancellationToken.None;

        _doRepoMock
            .Setup(x => x.DoLineUpdate(It.IsAny<DeliveryOrderLine>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DoLineUpdate(doLineVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("DO line updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task DoLineDelete_WithValidId_CommitsTransaction()
    {
        // Arrange
        var doLineId = 1;
        var cancellationToken = CancellationToken.None;

        _doRepoMock
            .Setup(x => x.DoLineDelete(doLineId, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DoLineDelete(doLineId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("DO line deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region Outstanding Tests

    [Fact]
    public async Task DoHdOsSelect_WithValidCustomerId_ReturnsOutstandingHeaders()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;
        var headers = new List<DeliveryOrderHeaderOutstanding>
        {
            new() { DoRecId = 1, DoId = "DO001", CustomerId = customerId }
        };

        _doRepoMock
            .Setup(x => x.DoHdOsSelect(customerId, cancellationToken))
            .ReturnsAsync(headers);

        // Act
        var result = await _sut.DoHdOsSelect(customerId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding DO headers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task DoOsSelect_WithValidCustomerId_ReturnsOutstandingLines()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;
        var lines = new List<DeliveryOrderLineOutstanding>
        {
            new() { DoLineId = 1, DoId = "DO001", ItemId = 1, QtyDo = 10 }
        };

        _doRepoMock
            .Setup(x => x.DoOsSelect(customerId, cancellationToken))
            .ReturnsAsync(lines);

        // Act
        var result = await _sut.DoOsSelect(customerId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding DO lines retrieved successfully.", result.Message);
    }

    #endregion

    #region ViewModel Tests

    [Fact]
    public async Task GetDeliveryOrderViewModel_WithValidRecId_ReturnsCompleteViewModel()
    {
        // Arrange
        const int recId = 1;
        var cancellationToken = CancellationToken.None;
        var header = new DeliveryOrderHeader { RecId = recId, DoId = "DO001", CustomerId = 1 };
        var lines = new List<DeliveryOrderLine>
        {
            new() { DoLineId = 1, DoRecId = recId, ItemId = 1 }
        };
        var outstanding = new List<DeliveryOrderLineOutstanding>
        {
            new() { DoLineId = 2, DoId = "DO002", ItemId = 2 }
        };

        _doRepoMock.Setup(x => x.DoSelectById(recId, cancellationToken))
            .ReturnsAsync(header);
        _doRepoMock.Setup(x => x.DoLineSelectByDo(recId, cancellationToken))
            .ReturnsAsync(lines);
        _doRepoMock.Setup(x => x.DoOsSelect(header.CustomerId, cancellationToken))
            .ReturnsAsync(outstanding);

        // Act
        var result = await _sut.GetDeliveryOrderViewModel(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Delivery Order ViewModel retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.NotNull(data.Header);
        Assert.NotNull(data.Lines);
        Assert.NotNull(data.Outstanding);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task DoInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var doVm = new DeliveryOrderVm 
        { 
            Header = new DeliveryOrderHeaderVm { CustomerId = 1 } 
        };
        var cancellationToken = CancellationToken.None;

        _doRepoMock
            .Setup(x => x.DoInsert(It.IsAny<DeliveryOrderHeader>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.DoInsert(doVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    #endregion
}





