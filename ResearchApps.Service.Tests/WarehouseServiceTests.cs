namespace ResearchApps.Service.Tests;

public class WarehouseServiceTests
{
    private readonly Mock<IWarehouseRepo> _warehouseRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<WarehouseService>> _loggerMock;
    private readonly WarehouseService _sut;

    public WarehouseServiceTests()
    {
        _warehouseRepoMock = new Mock<IWarehouseRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<WarehouseService>>();

        _sut = new WarehouseService(
            _warehouseRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
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
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(warehouses);

        // Act
        var result = await _sut.SelectAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouses retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsWarehouse()
    {
        // Arrange
        var whId = 1;
        var cancellationToken = CancellationToken.None;
        var warehouse = new Warehouse { WhId = whId, WhName = "Main Warehouse" };

        _warehouseRepoMock
            .Setup(x => x.SelectByIdAsync(whId, cancellationToken))
            .ReturnsAsync(warehouse);

        // Act
        var result = await _sut.SelectByIdAsync(whId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidWarehouse_ReturnsInsertedWarehouse()
    {
        // Arrange
        var warehouseVm = new WarehouseVm { WhName = "New Warehouse" };
        var cancellationToken = CancellationToken.None;
        var insertedWarehouse = new Warehouse { WhId = 3, WhName = "New Warehouse" };

        _warehouseRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Warehouse>(), cancellationToken))
            .ReturnsAsync(insertedWarehouse);

        // Act
        var result = await _sut.InsertAsync(warehouseVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var warehouseVm = new WarehouseVm { WhName = "New Warehouse" };
        var cancellationToken = CancellationToken.None;
        Warehouse? capturedWarehouse = null;

        _warehouseRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Warehouse>(), cancellationToken))
            .Callback<Warehouse, CancellationToken>((w, _) => capturedWarehouse = w)
            .ReturnsAsync(new Warehouse { WhId = 1 });

        // Act
        await _sut.InsertAsync(warehouseVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedWarehouse);
        Assert.Equal(_userClaimDto.Username, capturedWarehouse.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidWarehouse_ReturnsUpdatedWarehouse()
    {
        // Arrange
        var warehouseVm = new WarehouseVm { WhId = 1, WhName = "Updated Warehouse" };
        var cancellationToken = CancellationToken.None;
        var updatedWarehouse = new Warehouse { WhId = 1, WhName = "Updated Warehouse" };

        _warehouseRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Warehouse>(), cancellationToken))
            .ReturnsAsync(updatedWarehouse);

        // Act
        var result = await _sut.UpdateAsync(warehouseVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var warehouseVm = new WarehouseVm { WhId = 1, WhName = "Updated Warehouse" };
        var cancellationToken = CancellationToken.None;
        Warehouse? capturedWarehouse = null;

        _warehouseRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Warehouse>(), cancellationToken))
            .Callback<Warehouse, CancellationToken>((w, _) => capturedWarehouse = w)
            .ReturnsAsync(new Warehouse { WhId = 1 });

        // Act
        await _sut.UpdateAsync(warehouseVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedWarehouse);
        Assert.Equal(_userClaimDto.Username, capturedWarehouse.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        // Arrange
        var whId = 1;
        var modifiedBy = "admin";
        var cancellationToken = CancellationToken.None;

        _warehouseRepoMock
            .Setup(x => x.DeleteAsync(whId, modifiedBy, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(whId, modifiedBy, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_ReturnsWarehouseList()
    {
        // Arrange
        var warehouses = new List<Warehouse>
        {
            new() { WhId = 1, WhName = "Warehouse 1" },
            new() { WhId = 2, WhName = "Warehouse 2" }
        };

        _warehouseRepoMock
            .Setup(x => x.CboAsync())
            .ReturnsAsync(warehouses);

        // Act
        var result = await _sut.CboAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemTypes for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var warehouseVm = new WarehouseVm { WhName = "New Warehouse" };
        var cancellationToken = CancellationToken.None;

        _warehouseRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Warehouse>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(warehouseVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}





