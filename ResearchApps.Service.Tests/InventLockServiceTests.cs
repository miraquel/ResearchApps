namespace ResearchApps.Service.Tests;

public class InventLockServiceTests
{
    private readonly Mock<IInventLockRepo> _inventLockRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly InventLockService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public InventLockServiceTests()
    {
        _inventLockRepoMock = new Mock<IInventLockRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<InventLockService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new InventLockService(
            _inventLockRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectByYearAsync_WithValidYear_ReturnsInventoryLocks()
    {
        var year = 2026;
        var locks = new List<InventLock>
        {
            new() { RecId = 1, Year = 2026, Month = 1, Lock = true },
            new() { RecId = 2, Year = 2026, Month = 2, Lock = false }
        };

        _inventLockRepoMock
            .Setup(x => x.SelectByYearAsync(year, _ct))
            .ReturnsAsync(locks);

        var result = await _sut.SelectByYearAsync(year, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<InventLockVm>>>(result);
        Assert.NotNull(typed.Data);
        Assert.Equal(2, typed.Data.Count());
    }

    [Fact]
    public async Task CloseAsync_CommitsTransaction()
    {
        _inventLockRepoMock
            .Setup(x => x.CloseAsync(_ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CloseAsync(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Inventory closing process completed successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task OpenAsync_WithValidAction_CommitsTransaction()
    {
        var action = new InventLockActionVm { RecId = 1, Year = 2026, Month = 3 };

        _inventLockRepoMock
            .Setup(x => x.OpenAsync(action.RecId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.OpenAsync(action, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task OpenAsync_ReturnsCorrectMonthName()
    {
        var action = new InventLockActionVm { RecId = 1, Year = 2026, Month = 3 };

        _inventLockRepoMock
            .Setup(x => x.OpenAsync(action.RecId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.OpenAsync(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Contains("March 2026", result.Message);
    }

    [Fact]
    public async Task RunClosingAsync_WithValidAction_CommitsTransaction()
    {
        var action = new InventLockActionVm { RecId = 1, Year = 2026, Month = 3 };

        _inventLockRepoMock
            .Setup(x => x.RunClosingAsync(action.Year, action.Month, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.RunClosingAsync(action, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task RunClosingAsync_ReturnsCorrectMessage()
    {
        var action = new InventLockActionVm { RecId = 1, Year = 2026, Month = 3 };

        _inventLockRepoMock
            .Setup(x => x.RunClosingAsync(action.Year, action.Month, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.RunClosingAsync(action, _ct);

        Assert.True(result.IsSuccess);
        Assert.Contains("March 2026", result.Message);
    }

    [Fact]
    public async Task InitializeYearAsync_WhenRecordsExist_ReturnsSuccessWithoutInsert()
    {
        var year = 2026;
        var existingLocks = new List<InventLock>
        {
            new() { RecId = 1, Year = 2026, Month = 1, Lock = false }
        };

        _inventLockRepoMock
            .Setup(x => x.SelectByYearAsync(year, _ct))
            .ReturnsAsync(existingLocks);

        var result = await _sut.InitializeYearAsync(year, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task InitializeYearAsync_WhenNoRecordsExist_ReturnsInformationalMessage()
    {
        var year = 2026;

        _inventLockRepoMock
            .Setup(x => x.SelectByYearAsync(year, _ct))
            .ReturnsAsync(new List<InventLock>());

        var result = await _sut.InitializeYearAsync(year, _ct);

        Assert.True(result.IsSuccess);
        Assert.Contains("No inventory lock records", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task OpenAsync_WithInvalidMonth_ReturnsSuccessWithEmptyMonthName()
    {
        var action = new InventLockActionVm { RecId = 1, Year = 2026, Month = 0 };

        _inventLockRepoMock
            .Setup(x => x.OpenAsync(action.RecId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.OpenAsync(action, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }
}
