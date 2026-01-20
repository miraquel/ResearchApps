namespace ResearchApps.Service.Tests;

/// <summary>
/// Unit tests for CoService covering Customer Order operations
/// </summary>
public class CustomerOrderServiceTests
{
    private readonly Mock<ICustomerOrderRepo> _coRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly CustomerOrderService _sut;

    public CustomerOrderServiceTests()
    {
        _coRepoMock = new Mock<ICustomerOrderRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<CustomerOrderService>>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new CustomerOrderService(
            _coRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    #region CO Header Tests

    [Fact]
    public async Task CoSelect_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
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
            .Setup(x => x.CoSelect(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(cos);

        // Act
        var result = await _sut.CoSelect(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Orders retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoSelectById_WithValidId_ReturnsCo()
    {
        // Arrange
        var recId = 1;
        var cancellationToken = CancellationToken.None;
        var co = new CustomerOrderHeader { RecId = recId, CoId = "CO001", CustomerId = 1 };

        _coRepoMock
            .Setup(x => x.CoSelectById(recId, cancellationToken))
            .ReturnsAsync(co);

        // Act
        var result = await _sut.CoSelectById(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoInsert_WithValidCo_ReturnsInsertedIdAndCoId()
    {
        // Arrange
        var coVm = new CustomerOrderVm 
        { 
            Header = new CustomerOrderHeaderVm { CustomerId = 1, CoDate = DateTime.Now }
        };
        var cancellationToken = CancellationToken.None;
        var insertResult = (RecId: 10, CoId: "CO010");

        _coRepoMock
            .Setup(x => x.CoInsert(It.IsAny<CustomerOrderHeader>(), cancellationToken))
            .ReturnsAsync(insertResult);

        // Act
        var result = await _sut.CoInsert(coVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoInsert_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var coVm = new CustomerOrderVm 
        { 
            Header = new CustomerOrderHeaderVm { CustomerId = 1 }
        };
        var cancellationToken = CancellationToken.None;
        CustomerOrderHeader? capturedCo = null;

        _coRepoMock
            .Setup(x => x.CoInsert(It.IsAny<CustomerOrderHeader>(), cancellationToken))
            .Callback<CustomerOrderHeader, CancellationToken>((c, _) => capturedCo = c)
            .ReturnsAsync((1, "CO001"));

        // Act
        await _sut.CoInsert(coVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedCo);
        Assert.Equal(_userClaimDto.Username, capturedCo.CreatedBy);
    }

    [Fact]
    public async Task CoUpdate_WithValidCo_CommitsTransaction()
    {
        // Arrange
        var coVm = new CustomerOrderHeaderVm { RecId = 1, CoId = "CO001", CustomerId = 1 };
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoUpdate(It.IsAny<CustomerOrderHeader>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CoUpdate(coVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoDelete_WithValidId_CommitsTransaction()
    {
        // Arrange
        var recId = 1;
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoDelete(recId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CoDelete(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region CO Workflow Tests

    [Fact]
    public async Task CoSubmitById_WithValidId_CommitsTransaction()
    {
        // Arrange
        var action = new CustomerOrderWorkflowActionVm { RecId = 1 };
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoSubmitById(action.RecId, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CoSubmitById(action, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order submitted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoRecallById_WithValidId_CommitsTransaction()
    {
        // Arrange
        var action = new CustomerOrderWorkflowActionVm { RecId = 1 };
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoRecallById(action.RecId, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CoRecallById(action, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order recalled successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoRejectById_WithValidAction_CommitsTransaction()
    {
        // Arrange
        var action = new CustomerOrderWorkflowActionVm { RecId = 1, Notes = "Rejected" };
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoRejectById(action.RecId, _userClaimDto.Username, action.Notes, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CoRejectById(action, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order rejected successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoCloseByNo_WithValidCoId_CommitsTransaction()
    {
        // Arrange
        var action = new CustomerOrderWorkflowActionVm { CoId = "CO001" };
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoCloseByNo(action.CoId!, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CoCloseByNo(action, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer Order closed successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region CO Line Tests

    [Fact]
    public async Task CoLineSelectByCo_WithValidCoRecId_ReturnsCoLines()
    {
        // Arrange
        var coRecId = 1;
        var cancellationToken = CancellationToken.None;
        var coLines = new List<CustomerOrderLine>
        {
            new() { CoLineId = 1, CoRecId = coRecId, ItemId = 1 },
            new() { CoLineId = 2, CoRecId = coRecId, ItemId = 2 }
        };

        _coRepoMock
            .Setup(x => x.CoLineSelectByCo(coRecId, cancellationToken))
            .ReturnsAsync(coLines);

        // Act
        var result = await _sut.CoLineSelectByCo(coRecId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("CO lines retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoLineInsert_WithValidCoLine_ReturnsInsertedId()
    {
        // Arrange
        var coLineVm = new CustomerOrderLineVm { CoRecId = 1, ItemId = 1, Qty = 10 };
        var cancellationToken = CancellationToken.None;
        var insertedLineNo = "5";

        _coRepoMock
            .Setup(x => x.CoLineInsert(It.IsAny<CustomerOrderLine>(), cancellationToken))
            .ReturnsAsync(insertedLineNo);

        // Act
        var result = await _sut.CoLineInsert(coLineVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("CO line inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CoLineUpdate_WithValidCoLine_CommitsTransaction()
    {
        // Arrange
        var coLineVm = new CustomerOrderLineVm { CoLineId = 1, CoRecId = 1, ItemId = 1, Qty = 20 };
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoLineUpdate(It.IsAny<CustomerOrderLine>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CoLineUpdate(coLineVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("CO line updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region Outstanding Tests

    [Fact]
    public async Task CoHdOsSelect_WithValidCustomerId_ReturnsOutstandingHeaders()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;
        var headers = new List<CustomerOrderHeaderOutstanding>
        {
            new() { CoRecId = 1, CoId = "CO001", CustomerId = customerId }
        };

        _coRepoMock
            .Setup(x => x.CoHdOsSelect(customerId, cancellationToken))
            .ReturnsAsync(headers);

        // Act
        var result = await _sut.CoHdOsSelect(customerId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding CO headers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CoOsSelect_WithValidCustomerId_ReturnsOutstandingLines()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;
        var lines = new List<CustomerOrderLineOutstanding>
        {
            new() { CoLineId = 1, CoId = "CO001", ItemId = 1, QtyCo = 10 }
        };

        _coRepoMock
            .Setup(x => x.CoOsSelect(customerId, cancellationToken))
            .ReturnsAsync(lines);

        // Act
        var result = await _sut.CoOsSelect(customerId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Outstanding CO lines retrieved successfully.", result.Message);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task CoInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var coVm = new CustomerOrderVm 
        { 
            Header = new CustomerOrderHeaderVm { CustomerId = 1 }
        };
        var cancellationToken = CancellationToken.None;

        _coRepoMock
            .Setup(x => x.CoInsert(It.IsAny<CustomerOrderHeader>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CoInsert(coVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    #endregion
}



