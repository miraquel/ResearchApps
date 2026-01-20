namespace ResearchApps.Service.Tests;

public class ItemTypeServiceTests
{
    private readonly Mock<IItemTypeRepo> _itemTypeRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<ItemTypeService>> _loggerMock;
    private readonly ItemTypeService _sut;

    public ItemTypeServiceTests()
    {
        _itemTypeRepoMock = new Mock<IItemTypeRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<ItemTypeService>>();

        _sut = new ItemTypeService(
            _itemTypeRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ItemTypeSelectAsync_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
        var itemTypes = new PagedList<ItemType>(
            new List<ItemType>
            {
                new() { ItemTypeId = 1, ItemTypeName = "Type 1" },
                new() { ItemTypeId = 2, ItemTypeName = "Type 2" }
            },
            1,
            10,
            2
        );

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeSelectAsync(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(itemTypes);

        // Act
        var result = await _sut.ItemTypeSelectAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemTypes retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task ItemTypeSelectByIdAsync_WithValidId_ReturnsItemType()
    {
        // Arrange
        var itemTypeId = 1;
        var cancellationToken = CancellationToken.None;
        var itemType = new ItemType { ItemTypeId = itemTypeId, ItemTypeName = "Test Type" };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken))
            .ReturnsAsync(itemType);

        // Act
        var result = await _sut.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task ItemTypeInsertAsync_WithValidItemType_ReturnsInsertedItemType()
    {
        // Arrange
        var itemTypeVm = new ItemTypeVm { ItemTypeName = "New Type" };
        var cancellationToken = CancellationToken.None;
        var insertedItemType = new ItemType { ItemTypeId = 3, ItemTypeName = "New Type" };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeInsertAsync(It.IsAny<ItemType>(), cancellationToken))
            .ReturnsAsync(insertedItemType);

        // Act
        var result = await _sut.ItemTypeInsertAsync(itemTypeVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ItemTypeInsertAsync_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var itemTypeVm = new ItemTypeVm { ItemTypeName = "New Type" };
        var cancellationToken = CancellationToken.None;
        ItemType? capturedItemType = null;

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeInsertAsync(It.IsAny<ItemType>(), cancellationToken))
            .Callback<ItemType, CancellationToken>((it, _) => capturedItemType = it)
            .ReturnsAsync(new ItemType { ItemTypeId = 1 });

        // Act
        await _sut.ItemTypeInsertAsync(itemTypeVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedItemType);
        Assert.Equal(_userClaimDto.Username, capturedItemType.CreatedBy);
    }

    [Fact]
    public async Task ItemTypeUpdateAsync_WithValidItemType_ReturnsUpdatedItemType()
    {
        // Arrange
        var itemTypeVm = new ItemTypeVm { ItemTypeId = 1, ItemTypeName = "Updated Type" };
        var cancellationToken = CancellationToken.None;
        var updatedItemType = new ItemType { ItemTypeId = 1, ItemTypeName = "Updated Type" };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeUpdateAsync(It.IsAny<ItemType>(), cancellationToken))
            .ReturnsAsync(updatedItemType);

        // Act
        var result = await _sut.ItemTypeUpdateAsync(itemTypeVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ItemTypeUpdateAsync_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var itemTypeVm = new ItemTypeVm { ItemTypeId = 1, ItemTypeName = "Updated Type" };
        var cancellationToken = CancellationToken.None;
        ItemType? capturedItemType = null;

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeUpdateAsync(It.IsAny<ItemType>(), cancellationToken))
            .Callback<ItemType, CancellationToken>((it, _) => capturedItemType = it)
            .ReturnsAsync(new ItemType { ItemTypeId = 1 });

        // Act
        await _sut.ItemTypeUpdateAsync(itemTypeVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedItemType);
        Assert.Equal(_userClaimDto.Username, capturedItemType.ModifiedBy);
    }

    [Fact]
    public async Task ItemTypeDeleteAsync_WithValidId_CommitsTransaction()
    {
        // Arrange
        var itemTypeId = 1;
        var cancellationToken = CancellationToken.None;

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeDeleteAsync(itemTypeId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ItemTypeDeleteAsync(itemTypeId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ItemTypeCbo_WithValidRequest_ReturnsItemTypeList()
    {
        // Arrange
        var request = new CboRequestVm { Term = "Type" };
        var cancellationToken = CancellationToken.None;
        var itemTypes = new List<ItemType>
        {
            new() { ItemTypeId = 1, ItemTypeName = "Type 1" },
            new() { ItemTypeId = 2, ItemTypeName = "Type 2" }
        };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeCbo(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(itemTypes);

        // Act
        var result = await _sut.ItemTypeCbo(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemTypes for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task ItemTypeInsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var itemTypeVm = new ItemTypeVm { ItemTypeName = "New Type" };
        var cancellationToken = CancellationToken.None;

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeInsertAsync(It.IsAny<ItemType>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.ItemTypeInsertAsync(itemTypeVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}





