namespace ResearchApps.Service.Tests;

public class WarehouseServiceTests
{
    private readonly Mock<IWarehouseRepo> _warehouseRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly WarehouseService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public WarehouseServiceTests()
    {
        _warehouseRepoMock = new Mock<IWarehouseRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<WarehouseService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new WarehouseService(
            _warehouseRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var warehouses = new PagedList<Warehouse>(
            new List<Warehouse>
            {
                new() { WhId = 1, WhName = "Warehouse 1" },
                new() { WhId = 2, WhName = "Warehouse 2" }
            },
            1,
            10,
            2
        );

        _warehouseRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(warehouses);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouses retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsWarehouse()
    {
        var whId = 1;
        var warehouse = new Warehouse { WhId = whId, WhName = "Main Warehouse" };

        _warehouseRepoMock
            .Setup(x => x.SelectByIdAsync(whId, _ct))
            .ReturnsAsync(warehouse);

        var result = await _sut.SelectByIdAsync(whId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidWarehouse_ReturnsInsertedWarehouse()
    {
        var warehouseVm = new WarehouseVm { WhName = "New Warehouse" };
        var insertedWarehouse = new Warehouse { WhId = 3, WhName = "New Warehouse" };

        _warehouseRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Warehouse>(), _ct))
            .ReturnsAsync(insertedWarehouse);

        var result = await _sut.InsertAsync(warehouseVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var warehouseVm = new WarehouseVm { WhName = "New Warehouse" };
        Warehouse? capturedWarehouse = null;

        _warehouseRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Warehouse>(), _ct))
            .Callback<Warehouse, CancellationToken>((w, _) => capturedWarehouse = w)
            .ReturnsAsync(new Warehouse { WhId = 1 });

        await _sut.InsertAsync(warehouseVm, _ct);

        Assert.NotNull(capturedWarehouse);
        Assert.Equal(_userClaimDto.Username, capturedWarehouse.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidWarehouse_ReturnsUpdatedWarehouse()
    {
        var warehouseVm = new WarehouseVm { WhId = 1, WhName = "Updated Warehouse" };
        var updatedWarehouse = new Warehouse { WhId = 1, WhName = "Updated Warehouse" };

        _warehouseRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Warehouse>(), _ct))
            .ReturnsAsync(updatedWarehouse);

        var result = await _sut.UpdateAsync(warehouseVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var warehouseVm = new WarehouseVm { WhId = 1, WhName = "Updated Warehouse" };
        Warehouse? capturedWarehouse = null;

        _warehouseRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Warehouse>(), _ct))
            .Callback<Warehouse, CancellationToken>((w, _) => capturedWarehouse = w)
            .ReturnsAsync(new Warehouse { WhId = 1 });

        await _sut.UpdateAsync(warehouseVm, _ct);

        Assert.NotNull(capturedWarehouse);
        Assert.Equal(_userClaimDto.Username, capturedWarehouse.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        var whId = 1;
        var modifiedBy = "admin";

        _warehouseRepoMock
            .Setup(x => x.DeleteAsync(whId, modifiedBy, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(whId, modifiedBy, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_ReturnsWarehouseList()
    {
        var warehouses = new List<Warehouse>
        {
            new() { WhId = 1, WhName = "Warehouse 1" },
            new() { WhId = 2, WhName = "Warehouse 2" }
        };

        _warehouseRepoMock
            .Setup(x => x.CboAsync())
            .ReturnsAsync(warehouses);

        var result = await _sut.CboAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemTypes for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var warehouseVm = new WarehouseVm { WhName = "New Warehouse" };

        _warehouseRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Warehouse>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(warehouseVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
