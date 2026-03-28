namespace ResearchApps.Service.Tests;

public class BudgetServiceTests
{
    private readonly Mock<IBudgetRepo> _budgetRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly BudgetService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public BudgetServiceTests()
    {
        _budgetRepoMock = new Mock<IBudgetRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser", UserId = Guid.NewGuid() };
        var loggerMock = new Mock<ILogger<BudgetService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _sut = new BudgetService(_budgetRepoMock.Object, _dbTransactionMock.Object, _userClaimDto, loggerMock.Object);
    }

    [Fact]
    public async Task BudgetSelectAsync_WithValidRequest_ReturnsPagedBudgetList()
    {
        var request = new PagedListRequestVm { PageNumber = 2, PageSize = 5 };
        var budgets = new PagedList<Budget>(
            new List<Budget>
            {
                new() { BudgetId = 1, BudgetName = "Budget 1", Year = 2026 },
                new() { BudgetId = 2, BudgetName = "Budget 2", Year = 2026 }
            },
            2,
            5,
            12);

        _budgetRepoMock
            .Setup(x => x.BudgetSelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(budgets);

        var result = await _sut.BudgetSelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Budgets retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<PagedListVm<BudgetVm>>>(result);
        var data = Assert.IsType<PagedListVm<BudgetVm>>(typed.Data);
        Assert.Equal(2, data.PageNumber);
        Assert.Equal(5, data.PageSize);
        Assert.Equal(12, data.TotalCount);
        Assert.Equal(2, data.Items.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task BudgetSelectByIdAsync_WithExistingId_ReturnsBudget()
    {
        var budgetId = 10;
        var budget = new Budget
        {
            BudgetId = budgetId,
            BudgetName = "Capex 2026",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Amount = 100_000m,
            Year = 2026
        };

        _budgetRepoMock
            .Setup(x => x.BudgetSelectByIdAsync(budgetId, _ct))
            .ReturnsAsync(budget);

        var result = await _sut.BudgetSelectByIdAsync(budgetId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Budget retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<BudgetVm>>(result);
        var data = Assert.IsType<BudgetVm>(typed.Data);
        Assert.Equal(budgetId, data.BudgetId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task BudgetSelectByIdAsync_WithMissingId_ReturnsNotFound()
    {
        const int budgetId = 404;

        _budgetRepoMock
            .Setup(x => x.BudgetSelectByIdAsync(budgetId, _ct))
            .ReturnsAsync((Budget?)null);

        var result = await _sut.BudgetSelectByIdAsync(budgetId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.NotNull(result.Errors);
        Assert.Contains("Budget not found.", result.Errors);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task BudgetInsertAsync_WithValidBudget_ReturnsCreatedAndCommits()
    {
        var budgetVm = new BudgetVm
        {
            BudgetName = "Opex 2026",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Amount = 250_000m
        };

        Budget? capturedBudget = null;
        var insertedBudget = new Budget
        {
            BudgetId = 77,
            BudgetName = budgetVm.BudgetName,
            StartDate = budgetVm.StartDate,
            EndDate = budgetVm.EndDate,
            Amount = budgetVm.Amount,
            Year = budgetVm.StartDate.Year,
            StatusId = 1,
            CreatedBy = _userClaimDto.Username
        };

        _budgetRepoMock
            .Setup(x => x.BudgetInsertAsync(It.IsAny<Budget>(), _ct))
            .Callback<Budget, CancellationToken>((budget, _) => capturedBudget = budget)
            .ReturnsAsync(insertedBudget);

        var result = await _sut.BudgetInsertAsync(budgetVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Budget created successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<BudgetVm>>(result);
        var data = Assert.IsType<BudgetVm>(typed.Data);
        Assert.Equal(77, data.BudgetId);

        Assert.NotNull(capturedBudget);
        Assert.Equal(budgetVm.StartDate.Year, capturedBudget.Year);
        Assert.Equal(1, capturedBudget.StatusId);
        Assert.Equal(_userClaimDto.Username, capturedBudget.CreatedBy);

        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BudgetInsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var budgetVm = new BudgetVm
        {
            BudgetName = "Error Case",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Amount = 10_000m
        };

        _budgetRepoMock
            .Setup(x => x.BudgetInsertAsync(It.IsAny<Budget>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.BudgetInsertAsync(budgetVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task BudgetUpdateAsync_WithValidBudget_ReturnsUpdatedAndCommits()
    {
        var budgetVm = new BudgetVm
        {
            BudgetId = 5,
            BudgetName = "Opex 2027",
            StartDate = new DateTime(2027, 1, 1),
            EndDate = new DateTime(2027, 12, 31),
            Amount = 275_000m,
            StatusId = 2
        };

        Budget? capturedBudget = null;
        var updatedBudget = new Budget
        {
            BudgetId = budgetVm.BudgetId,
            BudgetName = budgetVm.BudgetName,
            StartDate = budgetVm.StartDate,
            EndDate = budgetVm.EndDate,
            Amount = budgetVm.Amount,
            Year = budgetVm.StartDate.Year,
            ModifiedBy = _userClaimDto.Username
        };

        _budgetRepoMock
            .Setup(x => x.BudgetUpdateAsync(It.IsAny<Budget>(), _ct))
            .Callback<Budget, CancellationToken>((budget, _) => capturedBudget = budget)
            .ReturnsAsync(updatedBudget);

        var result = await _sut.BudgetUpdateAsync(budgetVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Budget updated successfully.", result.Message);
        Assert.NotNull(capturedBudget);
        Assert.Equal(budgetVm.StartDate.Year, capturedBudget.Year);
        Assert.Equal(_userClaimDto.Username, capturedBudget.ModifiedBy);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BudgetUpdateAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var budgetVm = new BudgetVm
        {
            BudgetId = 5,
            BudgetName = "Error Update",
            StartDate = new DateTime(2027, 1, 1),
            EndDate = new DateTime(2027, 12, 31),
            Amount = 200_000m
        };

        _budgetRepoMock
            .Setup(x => x.BudgetUpdateAsync(It.IsAny<Budget>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.BudgetUpdateAsync(budgetVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task BudgetDeleteAsync_WithValidId_PassesUserAndCommits()
    {
        const int budgetId = 9;

        _budgetRepoMock
            .Setup(x => x.BudgetDeleteAsync(budgetId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.BudgetDeleteAsync(budgetId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Budget deleted successfully.", result.Message);
        _budgetRepoMock.Verify(x => x.BudgetDeleteAsync(budgetId, _userClaimDto.Username, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task BudgetDeleteAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        const int budgetId = 9;

        _budgetRepoMock
            .Setup(x => x.BudgetDeleteAsync(budgetId, _userClaimDto.Username, _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.BudgetDeleteAsync(budgetId, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task BudgetCboAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        var cboRequest = new CboRequestVm { Term = "Budget" };
        var budgets = new List<Budget>
        {
            new() { BudgetId = 1, BudgetName = "Budget 1", Amount = 10000 },
            new() { BudgetId = 2, BudgetName = "Budget 2", Amount = 20000 }
        };

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(budgets);

        var result = await _sut.BudgetCboAsync(cboRequest, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Budgets for combo box retrieved successfully.", result.Message);
        _budgetRepoMock.Verify(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), _ct), Times.Once);
    }

    [Fact]
    public async Task BudgetCboAsync_WithNullRequest_DoesNotThrowException()
    {
        CboRequestVm cboRequest = null!;

        await Assert.ThrowsAsync<NullReferenceException>(async () =>
            await _sut.BudgetCboAsync(cboRequest, _ct));
    }

    [Fact]
    public async Task BudgetCboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        var cboRequest = new CboRequestVm { Term = "NonExistent" };

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync([]);

        var result = await _sut.BudgetCboAsync(cboRequest, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<BudgetVm>>>(result);
        var data = Assert.IsType<BudgetVm[]>(typed.Data);
        Assert.NotNull(data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task BudgetCboAsync_WhenRepoThrowsException_PropagatesException()
    {
        var cboRequest = new CboRequestVm { Term = "Budget" };

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.BudgetCboAsync(cboRequest, _ct));
    }

    [Fact]
    public async Task BudgetCboAsync_WithCancellationToken_PassesTokenToRepo()
    {
        var cboRequest = new CboRequestVm { Term = "Budget" };
        var cancellationToken = new CancellationToken(true);

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ThrowsAsync(new TaskCanceledException());

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _sut.BudgetCboAsync(cboRequest, cancellationToken));
    }
}
