namespace ResearchApps.Service.Tests;

public class ItemDeptServiceTests
{
    private readonly Mock<IItemDeptRepo> _itemDeptRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ItemDeptService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ItemDeptServiceTests()
    {
        _itemDeptRepoMock = new Mock<IItemDeptRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<ItemDeptService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new ItemDeptService(
            _itemDeptRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
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
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(itemDepts);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDepts retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsItemDept()
    {
        var itemDeptId = 1;
        var itemDept = new ItemDept { ItemDeptId = itemDeptId, ItemDeptName = "Test Dept" };

        _itemDeptRepoMock
            .Setup(x => x.SelectByIdAsync(itemDeptId, _ct))
            .ReturnsAsync(itemDept);

        var result = await _sut.SelectByIdAsync(itemDeptId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Warehouse retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidItemDept_ReturnsInsertedItemDept()
    {
        var itemDeptVm = new ItemDeptVm { ItemDeptName = "New Dept" };
        var insertedItemDept = new ItemDept { ItemDeptId = 3, ItemDeptName = "New Dept" };

        _itemDeptRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemDept>(), _ct))
            .ReturnsAsync(insertedItemDept);

        var result = await _sut.InsertAsync(itemDeptVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDept inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var itemDeptVm = new ItemDeptVm { ItemDeptName = "New Dept" };
        ItemDept? capturedItemDept = null;

        _itemDeptRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemDept>(), _ct))
            .Callback<ItemDept, CancellationToken>((id, _) => capturedItemDept = id)
            .ReturnsAsync(new ItemDept { ItemDeptId = 1 });

        await _sut.InsertAsync(itemDeptVm, _ct);

        Assert.NotNull(capturedItemDept);
        Assert.Equal(_userClaimDto.Username, capturedItemDept.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidItemDept_ReturnsUpdatedItemDept()
    {
        var itemDeptVm = new ItemDeptVm { ItemDeptId = 1, ItemDeptName = "Updated Dept" };
        var updatedItemDept = new ItemDept { ItemDeptId = 1, ItemDeptName = "Updated Dept" };

        _itemDeptRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemDept>(), _ct))
            .ReturnsAsync(updatedItemDept);

        var result = await _sut.UpdateAsync(itemDeptVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDept updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var itemDeptVm = new ItemDeptVm { ItemDeptId = 1, ItemDeptName = "Updated Dept" };
        ItemDept? capturedItemDept = null;

        _itemDeptRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemDept>(), _ct))
            .Callback<ItemDept, CancellationToken>((id, _) => capturedItemDept = id)
            .ReturnsAsync(new ItemDept { ItemDeptId = 1 });

        await _sut.UpdateAsync(itemDeptVm, _ct);

        Assert.NotNull(capturedItemDept);
        Assert.Equal(_userClaimDto.Username, capturedItemDept.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        var itemDeptId = 1;
        var modifiedBy = "admin";

        _itemDeptRepoMock
            .Setup(x => x.DeleteAsync(itemDeptId, modifiedBy, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(itemDeptId, modifiedBy, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDept deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsItemDeptList()
    {
        var request = new CboRequestVm { Term = "Dept" };
        var itemDepts = new List<ItemDept>
        {
            new() { ItemDeptId = 1, ItemDeptName = "Dept 1" },
            new() { ItemDeptId = 2, ItemDeptName = "Dept 2" }
        };

        _itemDeptRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(itemDepts);

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemDepts for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var itemDeptVm = new ItemDeptVm { ItemDeptName = "New Dept" };

        _itemDeptRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemDept>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(itemDeptVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
