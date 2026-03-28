namespace ResearchApps.Service.Tests;

public class SalesPriceServiceTests
{
    private readonly Mock<ISalesPriceRepo> _salesPriceRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly SalesPriceService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public SalesPriceServiceTests()
    {
        _salesPriceRepoMock = new Mock<ISalesPriceRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<SalesPriceService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new SalesPriceService(
            _salesPriceRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var paged = new PagedList<SalesPrice>(
            new List<SalesPrice>
            {
                new() { RecId = 1 },
                new() { RecId = 2 }
            },
            1,
            10,
            2);

        _salesPriceRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(paged);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Prices retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalCount);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsSalesPrice()
    {
        const int recId = 1;
        var entity = new SalesPrice { RecId = recId };

        _salesPriceRepoMock
            .Setup(x => x.SelectByIdAsync(recId, _ct))
            .ReturnsAsync(entity);

        var result = await _sut.SelectByIdAsync(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Price retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(recId, result.Data.RecId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task InsertAsync_WithValidData_CommitsAndReturns201()
    {
        var vm = new SalesPriceVm { ItemId = 1, CustomerId = 2 };
        var inserted = new SalesPrice { RecId = 10 };

        _salesPriceRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<SalesPrice>(), _ct))
            .ReturnsAsync(inserted);

        var result = await _sut.InsertAsync(vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Sales Price created successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(10, result.Data.RecId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var vm = new SalesPriceVm { ItemId = 1, CustomerId = 2 };
        SalesPrice? captured = null;

        _salesPriceRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<SalesPrice>(), _ct))
            .Callback<SalesPrice, CancellationToken>((e, _) => captured = e)
            .ReturnsAsync(new SalesPrice { RecId = 1 });

        await _sut.InsertAsync(vm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_CommitsTransaction()
    {
        var vm = new SalesPriceVm { RecId = 5, ItemId = 1, CustomerId = 2 };
        var updated = new SalesPrice { RecId = 5 };

        _salesPriceRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<SalesPrice>(), _ct))
            .ReturnsAsync(updated);

        var result = await _sut.UpdateAsync(vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Price updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var vm = new SalesPriceVm { RecId = 5, ItemId = 1, CustomerId = 2 };
        SalesPrice? captured = null;

        _salesPriceRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<SalesPrice>(), _ct))
            .Callback<SalesPrice, CancellationToken>((e, _) => captured = e)
            .ReturnsAsync(new SalesPrice { RecId = 5 });

        await _sut.UpdateAsync(vm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        const int recId = 5;

        _salesPriceRepoMock
            .Setup(x => x.DeleteAsync(recId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Price deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrows_DoesNotCommit()
    {
        var vm = new SalesPriceVm { ItemId = 1, CustomerId = 2 };

        _salesPriceRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<SalesPrice>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.InsertAsync(vm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
