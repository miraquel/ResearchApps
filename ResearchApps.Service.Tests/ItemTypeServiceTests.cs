namespace ResearchApps.Service.Tests;

public class ItemTypeServiceTests
{
    private readonly Mock<IItemTypeRepo> _itemTypeRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ItemTypeService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ItemTypeServiceTests()
    {
        _itemTypeRepoMock = new Mock<IItemTypeRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<ItemTypeService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new ItemTypeService(
            _itemTypeRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task ItemTypeSelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
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
            .Setup(x => x.ItemTypeSelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(itemTypes);

        var result = await _sut.ItemTypeSelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemTypes retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task ItemTypeSelectByIdAsync_WithValidId_ReturnsItemType()
    {
        var itemTypeId = 1;
        var itemType = new ItemType { ItemTypeId = itemTypeId, ItemTypeName = "Test Type" };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeSelectByIdAsync(itemTypeId, _ct))
            .ReturnsAsync(itemType);

        var result = await _sut.ItemTypeSelectByIdAsync(itemTypeId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task ItemTypeInsertAsync_WithValidItemType_ReturnsInsertedItemType()
    {
        var itemTypeVm = new ItemTypeVm { ItemTypeName = "New Type" };
        var insertedItemType = new ItemType { ItemTypeId = 3, ItemTypeName = "New Type" };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeInsertAsync(It.IsAny<ItemType>(), _ct))
            .ReturnsAsync(insertedItemType);

        var result = await _sut.ItemTypeInsertAsync(itemTypeVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ItemTypeInsertAsync_SetsCreatedByFromUserClaim()
    {
        var itemTypeVm = new ItemTypeVm { ItemTypeName = "New Type" };
        ItemType? capturedItemType = null;

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeInsertAsync(It.IsAny<ItemType>(), _ct))
            .Callback<ItemType, CancellationToken>((it, _) => capturedItemType = it)
            .ReturnsAsync(new ItemType { ItemTypeId = 1 });

        await _sut.ItemTypeInsertAsync(itemTypeVm, _ct);

        Assert.NotNull(capturedItemType);
        Assert.Equal(_userClaimDto.Username, capturedItemType.CreatedBy);
    }

    [Fact]
    public async Task ItemTypeUpdateAsync_WithValidItemType_ReturnsUpdatedItemType()
    {
        var itemTypeVm = new ItemTypeVm { ItemTypeId = 1, ItemTypeName = "Updated Type" };
        var updatedItemType = new ItemType { ItemTypeId = 1, ItemTypeName = "Updated Type" };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeUpdateAsync(It.IsAny<ItemType>(), _ct))
            .ReturnsAsync(updatedItemType);

        var result = await _sut.ItemTypeUpdateAsync(itemTypeVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ItemTypeUpdateAsync_SetsModifiedByFromUserClaim()
    {
        var itemTypeVm = new ItemTypeVm { ItemTypeId = 1, ItemTypeName = "Updated Type" };
        ItemType? capturedItemType = null;

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeUpdateAsync(It.IsAny<ItemType>(), _ct))
            .Callback<ItemType, CancellationToken>((it, _) => capturedItemType = it)
            .ReturnsAsync(new ItemType { ItemTypeId = 1 });

        await _sut.ItemTypeUpdateAsync(itemTypeVm, _ct);

        Assert.NotNull(capturedItemType);
        Assert.Equal(_userClaimDto.Username, capturedItemType.ModifiedBy);
    }

    [Fact]
    public async Task ItemTypeDeleteAsync_WithValidId_CommitsTransaction()
    {
        var itemTypeId = 1;

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeDeleteAsync(itemTypeId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.ItemTypeDeleteAsync(itemTypeId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemType deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ItemTypeCbo_WithValidRequest_ReturnsItemTypeList()
    {
        var request = new CboRequestVm { Term = "Type" };
        var itemTypes = new List<ItemType>
        {
            new() { ItemTypeId = 1, ItemTypeName = "Type 1" },
            new() { ItemTypeId = 2, ItemTypeName = "Type 2" }
        };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeCbo(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(itemTypes);

        var result = await _sut.ItemTypeCbo(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemTypes for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task ItemTypeInsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var itemTypeVm = new ItemTypeVm { ItemTypeName = "New Type" };

        _itemTypeRepoMock
            .Setup(x => x.ItemTypeInsertAsync(It.IsAny<ItemType>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.ItemTypeInsertAsync(itemTypeVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
