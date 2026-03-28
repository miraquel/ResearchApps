namespace ResearchApps.Service.Tests;

public class TopServiceTests
{
    private readonly Mock<ITopRepo> _topRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly TopService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public TopServiceTests()
    {
        _topRepoMock = new Mock<ITopRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<TopService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new TopService(
            _topRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var tops = new PagedList<Top>(
            new List<Top>
            {
                new() { TopId = 1, TopName = "Top A" },
                new() { TopId = 2, TopName = "Top B" }
            },
            1,
            10,
            2
        );

        _topRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(tops);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("TOPs retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsTop()
    {
        const int topId = 1;
        var top = new Top { TopId = topId, TopName = "Top A" };

        _topRepoMock
            .Setup(x => x.SelectByIdAsync(topId, _ct))
            .ReturnsAsync(top);

        var result = await _sut.SelectByIdAsync(topId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("TOP retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidData_CommitsAndReturns201()
    {
        var topVm = new TopVm { TopName = "Top C" };
        var insertedTop = new Top { TopId = 3, TopName = "Top C" };

        _topRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Top>(), _ct))
            .ReturnsAsync(insertedTop);

        var result = await _sut.InsertAsync(topVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("TOP created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var topVm = new TopVm { TopName = "Top C" };
        Top? capturedTop = null;

        _topRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Top>(), _ct))
            .Callback<Top, CancellationToken>((t, _) => capturedTop = t)
            .ReturnsAsync(new Top { TopId = 1 });

        await _sut.InsertAsync(topVm, _ct);

        Assert.NotNull(capturedTop);
        Assert.Equal(_userClaimDto.Username, capturedTop.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_CommitsTransaction()
    {
        var topVm = new TopVm { TopId = 1, TopName = "Top A Updated" };
        var updatedTop = new Top { TopId = 1, TopName = "Top A Updated" };

        _topRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Top>(), _ct))
            .ReturnsAsync(updatedTop);

        var result = await _sut.UpdateAsync(topVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("TOP updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var topVm = new TopVm { TopId = 1, TopName = "Top A Updated" };
        Top? capturedTop = null;

        _topRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Top>(), _ct))
            .Callback<Top, CancellationToken>((t, _) => capturedTop = t)
            .ReturnsAsync(new Top { TopId = 1 });

        await _sut.UpdateAsync(topVm, _ct);

        Assert.NotNull(capturedTop);
        Assert.Equal(_userClaimDto.Username, capturedTop.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        const int topId = 1;

        _topRepoMock
            .Setup(x => x.DeleteAsync(topId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(topId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("TOP deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsTops()
    {
        var request = new CboRequestVm { Term = "Top" };
        var tops = new List<Top>
        {
            new() { TopId = 1, TopName = "Top A" },
            new() { TopId = 2, TopName = "Top B" }
        };

        _topRepoMock
            .Setup(x => x.CboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(tops);

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("TOPs for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrows_DoesNotCommit()
    {
        var topVm = new TopVm { TopName = "Top C" };

        _topRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Top>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(topVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
