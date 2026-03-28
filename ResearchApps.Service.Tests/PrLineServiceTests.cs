namespace ResearchApps.Service.Tests;

public class PrLineServiceTests
{
    private readonly Mock<IPrLineRepo> _prLineRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly PrLineService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public PrLineServiceTests()
    {
        _prLineRepoMock = new Mock<IPrLineRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<PrLineService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new PrLineService(
            _prLineRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task PrLineSelectByPr_WithValidPrId_ReturnsPrLines()
    {
        var prId = "PR001";
        var prLines = new List<PrLine>
        {
            new() { PrLineId = 1, PrId = prId, ItemId = 1 },
            new() { PrLineId = 2, PrId = prId, ItemId = 2 }
        };

        _prLineRepoMock
            .Setup(x => x.PrLineSelectByPr(prId, _ct))
            .ReturnsAsync(prLines);

        var result = await _sut.PrLineSelectByPr(prId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PrLines retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task PrLineSelectById_WithValidId_ReturnsPrLine()
    {
        var prLineId = 1;
        var prLine = new PrLine { PrLineId = prLineId, PrId = "PR001", ItemId = 1 };

        _prLineRepoMock
            .Setup(x => x.PrLineSelectById(prLineId, _ct))
            .ReturnsAsync(prLine);

        var result = await _sut.PrLineSelectById(prLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(prLineId, data.PrLineId);
    }

    [Fact]
    public async Task PrLineInsert_WithValidPrLine_ReturnsSuccessWithResult()
    {
        var prLineVm = new PrLineVm { PrId = "PR001", ItemId = 1, Qty = 10 };
        var insertResult = "PR001-1";

        _prLineRepoMock
            .Setup(x => x.PrLineInsert(It.IsAny<PrLine>(), _ct))
            .ReturnsAsync(insertResult);

        var result = await _sut.PrLineInsert(prLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrLineInsert_SetsCreatedByFromUserClaim()
    {
        var prLineVm = new PrLineVm { PrId = "PR001", ItemId = 1 };
        PrLine? capturedPrLine = null;

        _prLineRepoMock
            .Setup(x => x.PrLineInsert(It.IsAny<PrLine>(), _ct))
            .Callback<PrLine, CancellationToken>((pl, _) => capturedPrLine = pl)
            .ReturnsAsync("PR001-1");

        await _sut.PrLineInsert(prLineVm, _ct);

        Assert.NotNull(capturedPrLine);
        Assert.Equal(_userClaimDto.Username, capturedPrLine.CreatedBy);
    }

    [Fact]
    public async Task PrLineUpdate_WithValidPrLine_ReturnsSuccessWithResult()
    {
        var prLineVm = new PrLineVm { PrLineId = 1, PrId = "PR001", ItemId = 1, Qty = 20 };
        var updateResult = "PR001-1";

        _prLineRepoMock
            .Setup(x => x.PrLineUpdate(It.IsAny<PrLine>(), _ct))
            .ReturnsAsync(updateResult);

        var result = await _sut.PrLineUpdate(prLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine updated successfully.", result.Message);
        Assert.Equal(updateResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrLineUpdate_SetsModifiedByFromUserClaim()
    {
        var prLineVm = new PrLineVm { PrLineId = 1, PrId = "PR001", ItemId = 1 };
        PrLine? capturedPrLine = null;

        _prLineRepoMock
            .Setup(x => x.PrLineUpdate(It.IsAny<PrLine>(), _ct))
            .Callback<PrLine, CancellationToken>((pl, _) => capturedPrLine = pl)
            .ReturnsAsync("PR001-1");

        await _sut.PrLineUpdate(prLineVm, _ct);

        Assert.NotNull(capturedPrLine);
        Assert.Equal(_userClaimDto.Username, capturedPrLine.ModifiedBy);
    }

    [Fact]
    public async Task PrLineDelete_WithValidId_ReturnsSuccessWithResult()
    {
        var prLineId = 1;
        var deleteResult = "Deleted successfully";

        _prLineRepoMock
            .Setup(x => x.PrLineDelete(prLineId, _ct))
            .ReturnsAsync(deleteResult);

        var result = await _sut.PrLineDelete(prLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PrLine deleted successfully.", result.Message);
        Assert.Equal(deleteResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrLineInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var prLineVm = new PrLineVm { PrId = "PR001", ItemId = 1 };

        _prLineRepoMock
            .Setup(x => x.PrLineInsert(It.IsAny<PrLine>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.PrLineInsert(prLineVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PrLineSelectByPr_WithEmptyResults_ReturnsEmptyCollection()
    {
        var prId = "PR999";

        _prLineRepoMock
            .Setup(x => x.PrLineSelectByPr(prId, _ct))
            .ReturnsAsync(new List<PrLine>());

        var result = await _sut.PrLineSelectByPr(prId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task PrLineSelectForPo_WithValidParams_ReturnsAvailableLines()
    {
        const int poRecId = 1;
        var lines = new List<PrLine>
        {
            new() { PrLineId = 1, PrId = "PR001", ItemId = 10 },
            new() { PrLineId = 2, PrId = "PR001", ItemId = 20 }
        };

        _prLineRepoMock
            .Setup(x => x.PrLineSelectForPo(poRecId, 1, 10, null, null, null, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.PrLineSelectForPo(poRecId, 1, 10, null, null, null, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Available PR Lines fetched successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }
}
