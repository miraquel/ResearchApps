namespace ResearchApps.Service.Tests;

public class PrLineServiceTests
{
    private readonly Mock<IPrLineRepo> _prLineRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<PrLineService>> _loggerMock;
    private readonly PrLineService _sut;

    public PrLineServiceTests()
    {
        _prLineRepoMock = new Mock<IPrLineRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<PrLineService>>();

        _sut = new PrLineService(
            _prLineRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    [Fact]
    public async Task PrLineSelectByPr_WithValidPrId_ReturnsPrLines()
    {
        // Arrange
        var prId = "PR001";
        var cancellationToken = CancellationToken.None;
        var prLines = new List<PrLine>
        {
            new() { PrLineId = 1, PrId = prId, ItemId = 1 },
            new() { PrLineId = 2, PrId = prId, ItemId = 2 }
        };

        _prLineRepoMock
            .Setup(x => x.PrLineSelectByPr(prId, cancellationToken))
            .ReturnsAsync(prLines);

        // Act
        var result = await _sut.PrLineSelectByPr(prId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PrLines retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task PrLineSelectById_WithValidId_ReturnsPrLine()
    {
        // Arrange
        var prLineId = 1;
        var cancellationToken = CancellationToken.None;
        var prLine = new PrLine { PrLineId = prLineId, PrId = "PR001", ItemId = 1 };

        _prLineRepoMock
            .Setup(x => x.PrLineSelectById(prLineId, cancellationToken))
            .ReturnsAsync(prLine);

        // Act
        var result = await _sut.PrLineSelectById(prLineId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(prLineId, data.PrLineId);
    }

    [Fact]
    public async Task PrLineInsert_WithValidPrLine_ReturnsSuccessWithResult()
    {
        // Arrange
        var prLineVm = new PrLineVm { PrId = "PR001", ItemId = 1, Qty = 10 };
        var cancellationToken = CancellationToken.None;
        var insertResult = "PR001-1";

        _prLineRepoMock
            .Setup(x => x.PrLineInsert(It.IsAny<PrLine>(), cancellationToken))
            .ReturnsAsync(insertResult);

        // Act
        var result = await _sut.PrLineInsert(prLineVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrLineInsert_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var prLineVm = new PrLineVm { PrId = "PR001", ItemId = 1 };
        var cancellationToken = CancellationToken.None;
        PrLine? capturedPrLine = null;

        _prLineRepoMock
            .Setup(x => x.PrLineInsert(It.IsAny<PrLine>(), cancellationToken))
            .Callback<PrLine, CancellationToken>((pl, _) => capturedPrLine = pl)
            .ReturnsAsync("PR001-1");

        // Act
        await _sut.PrLineInsert(prLineVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedPrLine);
        Assert.Equal(_userClaimDto.Username, capturedPrLine.CreatedBy);
    }

    [Fact]
    public async Task PrLineUpdate_WithValidPrLine_ReturnsSuccessWithResult()
    {
        // Arrange
        var prLineVm = new PrLineVm { PrLineId = 1, PrId = "PR001", ItemId = 1, Qty = 20 };
        var cancellationToken = CancellationToken.None;
        var updateResult = "PR001-1";

        _prLineRepoMock
            .Setup(x => x.PrLineUpdate(It.IsAny<PrLine>(), cancellationToken))
            .ReturnsAsync(updateResult);

        // Act
        var result = await _sut.PrLineUpdate(prLineVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine updated successfully.", result.Message);
        Assert.Equal(updateResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrLineUpdate_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var prLineVm = new PrLineVm { PrLineId = 1, PrId = "PR001", ItemId = 1 };
        var cancellationToken = CancellationToken.None;
        PrLine? capturedPrLine = null;

        _prLineRepoMock
            .Setup(x => x.PrLineUpdate(It.IsAny<PrLine>(), cancellationToken))
            .Callback<PrLine, CancellationToken>((pl, _) => capturedPrLine = pl)
            .ReturnsAsync("PR001-1");

        // Act
        await _sut.PrLineUpdate(prLineVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedPrLine);
        Assert.Equal(_userClaimDto.Username, capturedPrLine.ModifiedBy);
    }

    [Fact]
    public async Task PrLineDelete_WithValidId_ReturnsSuccessWithResult()
    {
        // Arrange
        var prLineId = 1;
        var cancellationToken = CancellationToken.None;
        var deleteResult = "Deleted successfully";

        _prLineRepoMock
            .Setup(x => x.PrLineDelete(prLineId, cancellationToken))
            .ReturnsAsync(deleteResult);

        // Act
        var result = await _sut.PrLineDelete(prLineId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine deleted successfully.", result.Message);
        Assert.Equal(deleteResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrLineInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var prLineVm = new PrLineVm { PrId = "PR001", ItemId = 1 };
        var cancellationToken = CancellationToken.None;

        _prLineRepoMock
            .Setup(x => x.PrLineInsert(It.IsAny<PrLine>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.PrLineInsert(prLineVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PrLineSelectByPr_WithEmptyResults_ReturnsEmptyCollection()
    {
        // Arrange
        var prId = "PR999";
        var cancellationToken = CancellationToken.None;

        _prLineRepoMock
            .Setup(x => x.PrLineSelectByPr(prId, cancellationToken))
            .ReturnsAsync(new List<PrLine>());

        // Act
        var result = await _sut.PrLineSelectByPr(prId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }
}



