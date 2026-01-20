namespace ResearchApps.Service.Tests;

public class ItemDeptServiceTests
{
    private readonly Mock<IItemDeptRepo> _itemDeptRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<ItemDeptService>> _loggerMock;
    private readonly ItemDeptService _sut;

    public ItemDeptServiceTests()
    {
        _itemDeptRepoMock = new Mock<IItemDeptRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<ItemDeptService>>();

        _sut = new ItemDeptService(
            _itemDeptRepoMock.Object,
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
        var itemDepts = new PagedList<ItemDept>(
            new List<ItemDept>
            {
                new() { ItemDeptId = 1, ItemDeptName = "Dept 1" },
                new() { ItemDeptId = 2, ItemDeptName = "Dept 2" }
            },
            1,
            10,
            2
        );

        _itemDeptRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(itemDepts);

        // Act
        var result = await _sut.SelectAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDepts retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsItemDept()
    {
        // Arrange
        var itemDeptId = 1;
        var cancellationToken = CancellationToken.None;
        var itemDept = new ItemDept { ItemDeptId = itemDeptId, ItemDeptName = "Test Dept" };

        _itemDeptRepoMock
            .Setup(x => x.SelectByIdAsync(itemDeptId, cancellationToken))
            .ReturnsAsync(itemDept);

        // Act
        var result = await _sut.SelectByIdAsync(itemDeptId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidItemDept_ReturnsInsertedItemDept()
    {
        // Arrange
        var itemDeptVm = new ItemDeptVm { ItemDeptName = "New Dept" };
        var cancellationToken = CancellationToken.None;
        var insertedItemDept = new ItemDept { ItemDeptId = 3, ItemDeptName = "New Dept" };

        _itemDeptRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemDept>(), cancellationToken))
            .ReturnsAsync(insertedItemDept);

        // Act
        var result = await _sut.InsertAsync(itemDeptVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDept inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var itemDeptVm = new ItemDeptVm { ItemDeptName = "New Dept" };
        var cancellationToken = CancellationToken.None;
        ItemDept? capturedItemDept = null;

        _itemDeptRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemDept>(), cancellationToken))
            .Callback<ItemDept, CancellationToken>((id, _) => capturedItemDept = id)
            .ReturnsAsync(new ItemDept { ItemDeptId = 1 });

        // Act
        await _sut.InsertAsync(itemDeptVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedItemDept);
        Assert.Equal(_userClaimDto.Username, capturedItemDept.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidItemDept_ReturnsUpdatedItemDept()
    {
        // Arrange
        var itemDeptVm = new ItemDeptVm { ItemDeptId = 1, ItemDeptName = "Updated Dept" };
        var cancellationToken = CancellationToken.None;
        var updatedItemDept = new ItemDept { ItemDeptId = 1, ItemDeptName = "Updated Dept" };

        _itemDeptRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemDept>(), cancellationToken))
            .ReturnsAsync(updatedItemDept);

        // Act
        var result = await _sut.UpdateAsync(itemDeptVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDept updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var itemDeptVm = new ItemDeptVm { ItemDeptId = 1, ItemDeptName = "Updated Dept" };
        var cancellationToken = CancellationToken.None;
        ItemDept? capturedItemDept = null;

        _itemDeptRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemDept>(), cancellationToken))
            .Callback<ItemDept, CancellationToken>((id, _) => capturedItemDept = id)
            .ReturnsAsync(new ItemDept { ItemDeptId = 1 });

        // Act
        await _sut.UpdateAsync(itemDeptVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedItemDept);
        Assert.Equal(_userClaimDto.Username, capturedItemDept.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        // Arrange
        var itemDeptId = 1;
        var modifiedBy = "admin";
        var cancellationToken = CancellationToken.None;

        _itemDeptRepoMock
            .Setup(x => x.DeleteAsync(itemDeptId, modifiedBy, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(itemDeptId, modifiedBy, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDept deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsItemDeptList()
    {
        // Arrange
        var request = new CboRequestVm { Term = "Dept" };
        var cancellationToken = CancellationToken.None;
        var itemDepts = new List<ItemDept>
        {
            new() { ItemDeptId = 1, ItemDeptName = "Dept 1" },
            new() { ItemDeptId = 2, ItemDeptName = "Dept 2" }
        };

        _itemDeptRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(itemDepts);

        // Act
        var result = await _sut.CboAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDepts for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var itemDeptVm = new ItemDeptVm { ItemDeptName = "New Dept" };
        var cancellationToken = CancellationToken.None;

        _itemDeptRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemDept>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(itemDeptVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}





