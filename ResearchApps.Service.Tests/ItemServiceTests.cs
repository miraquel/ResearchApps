namespace ResearchApps.Service.Tests;

public class ItemServiceTests
{
    private readonly Mock<IItemRepo> _itemRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ItemService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ItemServiceTests()
    {
        _itemRepoMock = new Mock<IItemRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<ItemService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new ItemService(
            _itemRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var items = new PagedList<Item>(
            new List<Item>
            {
                new() { ItemId = 1, ItemName = "Item 1" },
                new() { ItemId = 2, ItemName = "Item 2" }
            },
            1,
            10,
            2
        );

        _itemRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(items);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Items retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsItem()
    {
        var itemId = 1;
        var item = new Item { ItemId = itemId, ItemName = "Test Item" };

        _itemRepoMock
            .Setup(x => x.SelectByIdAsync(itemId, _ct))
            .ReturnsAsync(item);

        var result = await _sut.SelectByIdAsync(itemId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Item retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<ItemVm>>(result);
        var data = Assert.IsType<ItemVm>(typed.Data);
        Assert.NotNull(data);
        Assert.Equal(itemId, data.ItemId);
    }

    [Fact]
    public async Task InsertAsync_WithValidItem_ReturnsInsertedItem()
    {
        var itemVm = new ItemVm { ItemName = "New Item", ItemTypeId = 1 };
        var insertedItem = new Item { ItemId = 10, ItemName = "New Item" };

        _itemRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Item>(), _ct))
            .ReturnsAsync(insertedItem);

        var result = await _sut.InsertAsync(itemVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Item inserted successfully.", result.Message);
        Assert.Equal(201, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var itemVm = new ItemVm { ItemName = "New Item" };
        Item? capturedItem = null;

        _itemRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Item>(), _ct))
            .Callback<Item, CancellationToken>((i, _) => capturedItem = i)
            .ReturnsAsync(new Item { ItemId = 1 });

        await _sut.InsertAsync(itemVm, _ct);

        Assert.NotNull(capturedItem);
        Assert.Equal(_userClaimDto.Username, capturedItem.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidItem_ReturnsUpdatedItem()
    {
        var itemVm = new ItemVm { ItemId = 1, ItemName = "Updated Item" };
        var updatedItem = new Item { ItemId = 1, ItemName = "Updated Item" };

        _itemRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Item>(), _ct))
            .ReturnsAsync(updatedItem);

        var result = await _sut.UpdateAsync(itemVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Item updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var itemVm = new ItemVm { ItemId = 1, ItemName = "Updated Item" };
        Item? capturedItem = null;

        _itemRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Item>(), _ct))
            .Callback<Item, CancellationToken>((i, _) => capturedItem = i)
            .ReturnsAsync(new Item { ItemId = 1 });

        await _sut.UpdateAsync(itemVm, _ct);

        Assert.NotNull(capturedItem);
        Assert.Equal(_userClaimDto.Username, capturedItem.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        var itemId = 1;

        _itemRepoMock
            .Setup(x => x.DeleteAsync(itemId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(itemId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Item deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsItemList()
    {
        var request = new CboRequestVm { Term = "Test" };
        var items = new List<Item>
        {
            new() { ItemId = 1, ItemName = "Test Item 1" },
            new() { ItemId = 2, ItemName = "Test Item 2" }
        };

        _itemRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(items);

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Items for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CboAsync_WithNullTerm_StillCallsRepo()
    {
        var request = new CboRequestVm { Term = null };

        _itemRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync([]);

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        _itemRepoMock.Verify(x => x.CboAsync(It.IsAny<CboRequest>(), _ct), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var itemVm = new ItemVm { ItemName = "New Item" };

        _itemRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Item>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(itemVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task SelectAsync_WithEmptyResults_ReturnsSuccessWithEmptyList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var items = new PagedList<Item>(
            new List<Item>(),
            1,
            10,
            0
        );

        _itemRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(items);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<PagedListVm<ItemVm>>>(result);
        var data = Assert.IsType<PagedListVm<ItemVm>>(typed.Data);
        Assert.NotNull(data);
        Assert.Empty(data.Items);
    }

    [Fact]
    public async Task CboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        var request = new CboRequestVm { Term = "NonExistent" };

        _itemRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(new List<Item>());

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<ItemVm>>>(result);
        var data = Assert.IsType<ItemVm[]>(typed.Data);
        Assert.NotNull(data);
        Assert.Empty(data);
    }
}
