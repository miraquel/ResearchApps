namespace ResearchApps.Service.Tests;

public class ItemServiceTests
{
    private readonly Mock<IItemRepo> _itemRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ItemService _sut;

    public ItemServiceTests()
    {
        _itemRepoMock = new Mock<IItemRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<ItemService>>();
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
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
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
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.SelectAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Items retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsItem()
    {
        // Arrange
        var itemId = 1;
        var cancellationToken = CancellationToken.None;
        var item = new Item { ItemId = itemId, ItemName = "Test Item" };

        _itemRepoMock
            .Setup(x => x.SelectByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(item);

        // Act
        var result = await _sut.SelectByIdAsync(itemId, cancellationToken);

        // Assert
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
        // Arrange
        var itemVm = new ItemVm { ItemName = "New Item", ItemTypeId = 1 };
        var cancellationToken = CancellationToken.None;
        var insertedItem = new Item { ItemId = 10, ItemName = "New Item" };

        _itemRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Item>(), cancellationToken))
            .ReturnsAsync(insertedItem);

        // Act
        var result = await _sut.InsertAsync(itemVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Item inserted successfully.", result.Message);
        Assert.Equal(201, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var itemVm = new ItemVm { ItemName = "New Item" };
        var cancellationToken = CancellationToken.None;
        Item? capturedItem = null;

        _itemRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Item>(), cancellationToken))
            .Callback<Item, CancellationToken>((i, _) => capturedItem = i)
            .ReturnsAsync(new Item { ItemId = 1 });

        // Act
        await _sut.InsertAsync(itemVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedItem);
        Assert.Equal(_userClaimDto.Username, capturedItem.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidItem_ReturnsUpdatedItem()
    {
        // Arrange
        var itemVm = new ItemVm { ItemId = 1, ItemName = "Updated Item" };
        var cancellationToken = CancellationToken.None;
        var updatedItem = new Item { ItemId = 1, ItemName = "Updated Item" };

        _itemRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Item>(), cancellationToken))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _sut.UpdateAsync(itemVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Item updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var itemVm = new ItemVm { ItemId = 1, ItemName = "Updated Item" };
        var cancellationToken = CancellationToken.None;
        Item? capturedItem = null;

        _itemRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Item>(), cancellationToken))
            .Callback<Item, CancellationToken>((i, _) => capturedItem = i)
            .ReturnsAsync(new Item { ItemId = 1 });

        // Act
        await _sut.UpdateAsync(itemVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedItem);
        Assert.Equal(_userClaimDto.Username, capturedItem.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        // Arrange
        var itemId = 1;
        var cancellationToken = CancellationToken.None;

        _itemRepoMock
            .Setup(x => x.DeleteAsync(itemId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(itemId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Item deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsItemList()
    {
        // Arrange
        var request = new CboRequestVm { Term = "Test" };
        var cancellationToken = CancellationToken.None;
        var items = new List<Item>
        {
            new() { ItemId = 1, ItemName = "Test Item 1" },
            new() { ItemId = 2, ItemName = "Test Item 2" }
        };

        _itemRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.CboAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Items for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CboAsync_WithNullTerm_StillCallsRepo()
    {
        // Arrange
        var request = new CboRequestVm { Term = null };
        var cancellationToken = CancellationToken.None;

        _itemRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.CboAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        _itemRepoMock.Verify(x => x.CboAsync(It.IsAny<CboRequest>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var itemVm = new ItemVm { ItemName = "New Item" };
        var cancellationToken = CancellationToken.None;

        _itemRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Item>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(itemVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task SelectAsync_WithEmptyResults_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
        var items = new PagedList<Item>(
            new List<Item>(),
            1,
            10,
            0
        );

        _itemRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.SelectAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<PagedListVm<ItemVm>>>(result);
        var data = Assert.IsType<PagedListVm<ItemVm>>(typed.Data);
        Assert.NotNull(data);
        Assert.Empty(data.Items);
    }

    [Fact]
    public async Task CboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        // Arrange
        var request = new CboRequestVm { Term = "NonExistent" };
        var cancellationToken = CancellationToken.None;

        _itemRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(new List<Item>());

        // Act
        var result = await _sut.CboAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<ItemVm>>>(result);
        var data = Assert.IsType<ItemVm[]>(typed.Data);
        Assert.NotNull(data);
        Assert.Empty(data);
    }
}



