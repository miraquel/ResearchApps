namespace ResearchApps.Service.Tests;

public class PoLineServiceTests
{
    private readonly Mock<IPoLineRepo> _poLineRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly PoLineService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public PoLineServiceTests()
    {
        _poLineRepoMock = new Mock<IPoLineRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<PoLineService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new PoLineService(
            _poLineRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task PoLineSelectById_WithExistingId_ReturnsPoLine()
    {
        var poLineId = 1;
        var poLine = new PoLine
        {
            PoLineId = poLineId,
            PoId = "PO001",
            ItemId = 1,
            Qty = 10,
            Price = 100
        };

        _poLineRepoMock
            .Setup(x => x.PoLineSelectById(poLineId, _ct))
            .ReturnsAsync(poLine);

        var result = await _sut.PoLineSelectById(poLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("PO001", result.Data.PoId);
    }

    [Fact]
    public async Task PoLineSelectById_WithNonExistingId_ReturnsNotFound()
    {
        var poLineId = 999;

        _poLineRepoMock
            .Setup(x => x.PoLineSelectById(poLineId, _ct))
            .ReturnsAsync((PoLine?)null);

        var result = await _sut.PoLineSelectById(poLineId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task PoLineSelectByPo_WithValidPoId_ReturnsLines()
    {
        const int poRecId = 1;
        var poLines = new List<PoLine>
        {
            new() { PoLineId = 1, RecId = poRecId, ItemId = 1 },
            new() { PoLineId = 2, RecId = poRecId, ItemId = 2 }
        };

        _poLineRepoMock
            .Setup(x => x.PoLineSelectByPo(poRecId, _ct))
            .ReturnsAsync(poLines);

        var result = await _sut.PoLineSelectByPo(poRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task PoLineInsert_WithValidData_ReturnsPoIdAndCommits()
    {
        var poLineVm = new PoLineVm
        {
            PoId = "PO001",
            ItemId = 1,
            Qty = 10,
            Price = 100,
            UnitId = 1
        };

        _poLineRepoMock
            .Setup(x => x.PoLineInsert(It.IsAny<PoLine>(), _ct))
            .ReturnsAsync("PO001");

        var result = await _sut.PoLineInsert(poLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("PO001", result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoLineDelete_WithValidId_ReturnsPoIdAndCommits()
    {
        var poLineId = 1;
        var poLine = new PoLine { PoLineId = poLineId, PoId = "PO001" };

        _poLineRepoMock
            .Setup(x => x.PoLineSelectById(poLineId, _ct))
            .ReturnsAsync(poLine);

        _poLineRepoMock
            .Setup(x => x.PoLineDelete(poLineId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PoLineDelete(poLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PO001", result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoLineInsert_SetsAuditFields()
    {
        var poLineVm = new PoLineVm
        {
            PoId = "PO001",
            ItemId = 1,
            Qty = 10,
            Price = 100,
            UnitId = 1
        };

        PoLine? capturedEntity = null;
        _poLineRepoMock
            .Setup(x => x.PoLineInsert(It.IsAny<PoLine>(), _ct))
            .Callback<PoLine, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync("PO001");

        await _sut.PoLineInsert(poLineVm, _ct);

        Assert.NotNull(capturedEntity);
        Assert.Equal("testuser", capturedEntity.CreatedBy);
        Assert.Equal("testuser", capturedEntity.ModifiedBy);
    }

    [Fact]
    public async Task PoLineUpdate_WithValidData_ReturnsUpdatedLineAndCommits()
    {
        var poLineVm = new PoLineVm
        {
            PoLineId = 1,
            PoId = "PO001",
            ItemId = 1,
            Qty = 20,
            Price = 100
        };

        var updatedLine = new PoLine { PoLineId = 1, PoId = "PO001", ItemId = 1, Qty = 20, Price = 100 };

        _poLineRepoMock
            .Setup(x => x.PoLineUpdate(It.IsAny<PoLine>(), _ct))
            .Returns(Task.CompletedTask);

        _poLineRepoMock
            .Setup(x => x.PoLineSelectById(1, _ct))
            .ReturnsAsync(updatedLine);

        var result = await _sut.PoLineUpdate(poLineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("PO line updated successfully.", result.Message);
        Assert.NotNull(result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoLineDelete_WithNullLine_ReturnsEmptyPoId()
    {
        var poLineId = 5;

        _poLineRepoMock
            .Setup(x => x.PoLineSelectById(poLineId, _ct))
            .ReturnsAsync((PoLine?)null);

        _poLineRepoMock
            .Setup(x => x.PoLineDelete(poLineId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PoLineDelete(poLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }
}
