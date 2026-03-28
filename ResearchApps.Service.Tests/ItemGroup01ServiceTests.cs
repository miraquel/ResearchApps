namespace ResearchApps.Service.Tests;

public class ItemGroup01ServiceTests
{
    private readonly Mock<IItemGroup01Repo> _itemGroup01RepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ItemGroup01Service _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ItemGroup01ServiceTests()
    {
        _itemGroup01RepoMock = new Mock<IItemGroup01Repo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<ItemGroup01Service>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new ItemGroup01Service(
            _itemGroup01RepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsItemGroup01List()
    {
        var request = new CboRequestVm { Term = "Group" };
        var itemGroup01s = new List<ItemGroup01>
        {
            new() { ItemGroup01Id = 1, ItemGroup01Name = "Group 1" },
            new() { ItemGroup01Id = 2, ItemGroup01Name = "Group 2" }
        };

        _itemGroup01RepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(itemGroup01s);

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup01s for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsItemGroup01()
    {
        var itemGroup01Id = 1;
        var itemGroup01 = new ItemGroup01 { ItemGroup01Id = itemGroup01Id, ItemGroup01Name = "Test Group" };

        _itemGroup01RepoMock
            .Setup(x => x.SelectByIdAsync(itemGroup01Id, _ct))
            .ReturnsAsync(itemGroup01);

        var result = await _sut.SelectByIdAsync(itemGroup01Id, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup01 retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var itemGroup01s = new PagedList<ItemGroup01>(
            new List<ItemGroup01>
            {
                new() { ItemGroup01Id = 1, ItemGroup01Name = "Group 1" },
                new() { ItemGroup01Id = 2, ItemGroup01Name = "Group 2" }
            },
            1,
            10,
            2
        );

        _itemGroup01RepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(itemGroup01s);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup01s retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidItemGroup01_ReturnsInsertedItemGroup01()
    {
        var itemGroup01Vm = new ItemGroup01Vm { ItemGroup01Name = "New Group" };
        var insertedItemGroup01 = new ItemGroup01 { ItemGroup01Id = 3, ItemGroup01Name = "New Group" };

        _itemGroup01RepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemGroup01>(), _ct))
            .ReturnsAsync(insertedItemGroup01);

        var result = await _sut.InsertAsync(itemGroup01Vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup01 inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var itemGroup01Vm = new ItemGroup01Vm { ItemGroup01Name = "New Group" };
        ItemGroup01? capturedItemGroup01 = null;

        _itemGroup01RepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemGroup01>(), _ct))
            .Callback<ItemGroup01, CancellationToken>((e, _) => capturedItemGroup01 = e)
            .ReturnsAsync(new ItemGroup01 { ItemGroup01Id = 1 });

        await _sut.InsertAsync(itemGroup01Vm, _ct);

        Assert.NotNull(capturedItemGroup01);
        Assert.Equal(_userClaimDto.Username, capturedItemGroup01.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidItemGroup01_ReturnsUpdatedItemGroup01()
    {
        var itemGroup01Vm = new ItemGroup01Vm { ItemGroup01Id = 1, ItemGroup01Name = "Updated Group" };
        var updatedItemGroup01 = new ItemGroup01 { ItemGroup01Id = 1, ItemGroup01Name = "Updated Group" };

        _itemGroup01RepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemGroup01>(), _ct))
            .ReturnsAsync(updatedItemGroup01);

        var result = await _sut.UpdateAsync(itemGroup01Vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup01 updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var itemGroup01Vm = new ItemGroup01Vm { ItemGroup01Id = 1, ItemGroup01Name = "Updated Group" };
        ItemGroup01? capturedItemGroup01 = null;

        _itemGroup01RepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ItemGroup01>(), _ct))
            .Callback<ItemGroup01, CancellationToken>((e, _) => capturedItemGroup01 = e)
            .ReturnsAsync(new ItemGroup01 { ItemGroup01Id = 1 });

        await _sut.UpdateAsync(itemGroup01Vm, _ct);

        Assert.NotNull(capturedItemGroup01);
        Assert.Equal(_userClaimDto.Username, capturedItemGroup01.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        var itemGroup01Id = 1;
        var modifiedBy = "admin";

        _itemGroup01RepoMock
            .Setup(x => x.DeleteAsync(itemGroup01Id, modifiedBy, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(itemGroup01Id, modifiedBy, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("ItemGroup01 deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var itemGroup01Vm = new ItemGroup01Vm { ItemGroup01Name = "New Group" };

        _itemGroup01RepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ItemGroup01>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(itemGroup01Vm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
