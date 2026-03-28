namespace ResearchApps.Service.Tests;

public class ItemGroup02ServiceTests
{
    private readonly Mock<IItemGroup02Repo> _itemGroup02RepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ItemGroup02Service _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ItemGroup02ServiceTests()
    {
        _itemGroup02RepoMock = new Mock<IItemGroup02Repo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<ItemGroup02Service>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new ItemGroup02Service(
            _itemGroup02RepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsItemGroup02List()
    {
        var request = new CboRequestVm { Term = "Group" };
        var itemGroup02s = new List<ItemGroup02>
        {
            new() { ItemGroup02Id = 1, ItemGroup02Name = "Group 1" },
            new() { ItemGroup02Id = 2, ItemGroup02Name = "Group 2" }
        };

        _itemGroup02RepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(itemGroup02s);

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup02s for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsItemGroup02()
    {
        var itemGroup02Id = 1;
        var itemGroup02 = new ItemGroup02 { ItemGroup02Id = itemGroup02Id, ItemGroup02Name = "Test Group" };

        _itemGroup02RepoMock
            .Setup(x => x.SelectByIdAsync(itemGroup02Id, _ct))
            .ReturnsAsync(itemGroup02);

        var result = await _sut.SelectByIdAsync(itemGroup02Id, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup02 retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var itemGroup02s = new PagedList<ItemGroup02>(
            new List<ItemGroup02>
            {
                new() { ItemGroup02Id = 1, ItemGroup02Name = "Group 1" },
                new() { ItemGroup02Id = 2, ItemGroup02Name = "Group 2" }
            },
            1,
            10,
            2
        );

        _itemGroup02RepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(itemGroup02s);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup02s retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidItemGroup02_ReturnsInsertedItemGroup02()
    {
        var itemGroup02Vm = new ItemGroup02Vm { ItemGroup02Name = "New Group" };
        var insertedItemGroup02 = new ItemGroup02 { ItemGroup02Id = 3, ItemGroup02Name = "New Group" };

        _itemGroup02RepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemGroup02>(), _ct))
            .ReturnsAsync(insertedItemGroup02);

        var result = await _sut.InsertAsync(itemGroup02Vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup02 inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var itemGroup02Vm = new ItemGroup02Vm { ItemGroup02Name = "New Group" };
        ItemGroup02? capturedItemGroup02 = null;

        _itemGroup02RepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemGroup02>(), _ct))
            .Callback<ItemGroup02, CancellationToken>((e, _) => capturedItemGroup02 = e)
            .ReturnsAsync(new ItemGroup02 { ItemGroup02Id = 1 });

        await _sut.InsertAsync(itemGroup02Vm, _ct);

        Assert.NotNull(capturedItemGroup02);
        Assert.Equal(_userClaimDto.Username, capturedItemGroup02.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidItemGroup02_ReturnsUpdatedItemGroup02()
    {
        var itemGroup02Vm = new ItemGroup02Vm { ItemGroup02Id = 1, ItemGroup02Name = "Updated Group" };
        var updatedItemGroup02 = new ItemGroup02 { ItemGroup02Id = 1, ItemGroup02Name = "Updated Group" };

        _itemGroup02RepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemGroup02>(), _ct))
            .ReturnsAsync(updatedItemGroup02);

        var result = await _sut.UpdateAsync(itemGroup02Vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup02 updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var itemGroup02Vm = new ItemGroup02Vm { ItemGroup02Id = 1, ItemGroup02Name = "Updated Group" };
        ItemGroup02? capturedItemGroup02 = null;

        _itemGroup02RepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemGroup02>(), _ct))
            .Callback<ItemGroup02, CancellationToken>((e, _) => capturedItemGroup02 = e)
            .ReturnsAsync(new ItemGroup02 { ItemGroup02Id = 1 });

        await _sut.UpdateAsync(itemGroup02Vm, _ct);

        Assert.NotNull(capturedItemGroup02);
        Assert.Equal(_userClaimDto.Username, capturedItemGroup02.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        var itemGroup02Id = 1;
        var modifiedBy = "admin";

        _itemGroup02RepoMock
            .Setup(x => x.DeleteAsync(itemGroup02Id, modifiedBy, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(itemGroup02Id, modifiedBy, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup02 deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var itemGroup02Vm = new ItemGroup02Vm { ItemGroup02Name = "New Group" };

        _itemGroup02RepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemGroup02>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(itemGroup02Vm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
