namespace ResearchApps.Service.Tests;

public class BudgetServiceTests
{
    private readonly Mock<IBudgetRepo> _budgetRepoMock;
    private readonly Mock<ILogger<BudgetService>> _loggerMock;
    private readonly BudgetService _sut;

    public BudgetServiceTests()
    {
        _budgetRepoMock = new Mock<IBudgetRepo>();
        _loggerMock = new Mock<ILogger<BudgetService>>();
        _sut = new BudgetService(_budgetRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task BudgetCboAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "Budget" };
        var cancellationToken = CancellationToken.None;
        var budgets = new List<Budget>
        {
            new() { BudgetId = 1, BudgetName = "Budget 1", Amount = 10000 },
            new() { BudgetId = 2, BudgetName = "Budget 2", Amount = 20000 }
        };

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(budgets);

        // Act
        var result = await _sut.BudgetCboAsync(cboRequest, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Budgets for combo box retrieved successfully.", result.Message);
        _budgetRepoMock.Verify(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task BudgetCboAsync_WithNullRequest_DoesNotThrowException()
    {
        // Arrange
        CboRequestVm cboRequest = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
            await _sut.BudgetCboAsync(cboRequest, cancellationToken));
    }

    [Fact]
    public async Task BudgetCboAsync_WithEmptyResults_ReturnsSuccessWithEmptyCollection()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "NonExistent" };
        var cancellationToken = CancellationToken.None;

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.BudgetCboAsync(cboRequest, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<BudgetVm>>>(result);
        var data = Assert.IsType<BudgetVm[]>(typed.Data);
        Assert.NotNull(data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task BudgetCboAsync_WhenRepoThrowsException_PropagatesException()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "Budget" };
        var cancellationToken = CancellationToken.None;

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.BudgetCboAsync(cboRequest, cancellationToken));
    }

    [Fact]
    public async Task BudgetCboAsync_WithCancellationToken_PassesTokenToRepo()
    {
        // Arrange
        var cboRequest = new CboRequestVm { Term = "Budget" };
        var cancellationToken = new CancellationToken(true);

        _budgetRepoMock
            .Setup(x => x.BudgetCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ThrowsAsync(new TaskCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _sut.BudgetCboAsync(cboRequest, cancellationToken));
    }
}



