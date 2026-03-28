namespace ResearchApps.Service.Tests;

public class PsServiceTests
{
    private readonly Mock<IPsRepo> _psRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly PsService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public PsServiceTests()
    {
        _psRepoMock = new Mock<IPsRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<PsService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new PsService(
            _psRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task PsSelect_WithoutPaging_ReturnsListSuccessfully()
    {
        var pss = new List<PsHeader>
        {
            new() { RecId = 1, PsId = "PS001" },
            new() { RecId = 2, PsId = "PS002" }
        };

        _psRepoMock
            .Setup(x => x.PsSelect(_ct))
            .ReturnsAsync(pss);

        var result = await _sut.PsSelect(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock list retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PsSelect_WithPaging_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var paged = new PagedList<PsHeader>(
            new List<PsHeader>
            {
                new() { RecId = 1, PsId = "PS001" },
                new() { RecId = 2, PsId = "PS002" }
            },
            1,
            10,
            2);

        _psRepoMock
            .Setup(x => x.PsSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(paged);

        var result = await _sut.PsSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock list retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalCount);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PsSelectById_WithValidId_ReturnsPs()
    {
        const int recId = 1;
        var ps = new PsHeader { RecId = recId, PsId = "PS001" };

        _psRepoMock
            .Setup(x => x.PsSelectById(recId, _ct))
            .ReturnsAsync(ps);

        var result = await _sut.PsSelectById(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("PS001", result.Data.PsId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PsInsert_WithValidPs_CommitsAndReturns201()
    {
        var psVm = new PsVm
        {
            Header = new PsHeaderVm(),
            Lines = new List<PsLineVm>()
        };

        _psRepoMock
            .Setup(x => x.PsInsert(It.IsAny<PsHeader>(), _ct))
            .ReturnsAsync((RecId: 1, PsId: "PS001"));

        var result = await _sut.PsInsert(psVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Penyesuaian Stock created successfully.", result.Message);
        Assert.Equal(1, result.Data.RecId);
        Assert.Equal("PS001", result.Data.PsId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PsInsert_SetsCreatedByFromUserClaim()
    {
        var psVm = new PsVm
        {
            Header = new PsHeaderVm(),
            Lines = new List<PsLineVm>()
        };
        PsHeader? captured = null;

        _psRepoMock
            .Setup(x => x.PsInsert(It.IsAny<PsHeader>(), _ct))
            .Callback<PsHeader, CancellationToken>((h, _) => captured = h)
            .ReturnsAsync((RecId: 1, PsId: "PS001"));

        await _sut.PsInsert(psVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task PsUpdate_WithValidPs_CommitsTransaction()
    {
        var headerVm = new PsHeaderVm { RecId = 1, PsId = "PS001" };

        _psRepoMock
            .Setup(x => x.PsUpdate(It.IsAny<PsHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PsUpdate(headerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PsUpdate_SetsModifiedByFromUserClaim()
    {
        var headerVm = new PsHeaderVm { RecId = 1, PsId = "PS001" };
        PsHeader? captured = null;

        _psRepoMock
            .Setup(x => x.PsUpdate(It.IsAny<PsHeader>(), _ct))
            .Callback<PsHeader, CancellationToken>((h, _) => captured = h)
            .Returns(Task.CompletedTask);

        await _sut.PsUpdate(headerVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.ModifiedBy);
    }

    [Fact]
    public async Task PsDelete_WithValidId_CommitsTransaction()
    {
        const int recId = 1;

        _psRepoMock
            .Setup(x => x.PsDelete(recId, _userClaimDto.Username, _ct))
            .ReturnsAsync("1");

        var result = await _sut.PsDelete(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PsLineSelectByPs_WithValidId_ReturnsLines()
    {
        const int psRecId = 1;
        var lines = new List<PsLine>
        {
            new() { PsLineId = 1, PsRecId = psRecId },
            new() { PsLineId = 2, PsRecId = psRecId }
        };

        _psRepoMock
            .Setup(x => x.PsLineSelectByPs(psRecId, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.PsLineSelectByPs(psRecId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock lines retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PsLineSelectById_WithValidId_ReturnsLine()
    {
        const int psLineId = 1;
        var line = new PsLine { PsLineId = psLineId, PsRecId = 1 };

        _psRepoMock
            .Setup(x => x.PsLineSelectById(psLineId, _ct))
            .ReturnsAsync(line);

        var result = await _sut.PsLineSelectById(psLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock line retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(psLineId, result.Data.PsLineId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PsLineSelectById_WithInvalidId_ReturnsNotFound()
    {
        const int psLineId = 999;

        _psRepoMock
            .Setup(x => x.PsLineSelectById(psLineId, _ct))
            .ReturnsAsync((PsLine?)null);

        var result = await _sut.PsLineSelectById(psLineId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task PsLineInsert_WithSuccess_CommitsAndReturns201()
    {
        var lineVm = new PsLineVm { PsRecId = 1, ItemId = 10, Qty = 5 };

        _psRepoMock
            .Setup(x => x.PsLineInsert(It.IsAny<PsLine>(), _ct))
            .ReturnsAsync("5");

        var result = await _sut.PsLineInsert(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Penyesuaian Stock line inserted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PsLineInsert_WithErrorResponse_ReturnsFailure400()
    {
        var lineVm = new PsLineVm { PsRecId = 1, ItemId = 10, Qty = 5 };

        _psRepoMock
            .Setup(x => x.PsLineInsert(It.IsAny<PsLine>(), _ct))
            .ReturnsAsync("-1:::Stock not available");

        var result = await _sut.PsLineInsert(lineVm, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PsLineUpdate_WithValidLine_CommitsTransaction()
    {
        var lineVm = new PsLineVm { PsLineId = 1, PsRecId = 1, ItemId = 10, Qty = 10 };

        _psRepoMock
            .Setup(x => x.PsLineUpdate(It.IsAny<PsLine>(), _ct))
            .ReturnsAsync("ok");

        var result = await _sut.PsLineUpdate(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock line updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PsLineDelete_WithSuccess_CommitsTransaction()
    {
        const int psLineId = 1;

        _psRepoMock
            .Setup(x => x.PsLineDelete(psLineId, _userClaimDto.Username, _ct))
            .ReturnsAsync("1");

        var result = await _sut.PsLineDelete(psLineId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock line deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PsLineDelete_WithErrorResponse_ReturnsFailure400()
    {
        const int psLineId = 1;

        _psRepoMock
            .Setup(x => x.PsLineDelete(psLineId, _userClaimDto.Username, _ct))
            .ReturnsAsync("-1:::Cannot delete");

        var result = await _sut.PsLineDelete(psLineId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetPs_WithValidId_ReturnsCompositeVm()
    {
        const int recId = 1;
        var header = new PsHeader { RecId = recId, PsId = "PS001" };
        var lines = new List<PsLine>
        {
            new() { PsLineId = 1, PsRecId = recId },
            new() { PsLineId = 2, PsRecId = recId }
        };

        _psRepoMock.Setup(x => x.PsSelectById(recId, _ct)).ReturnsAsync(header);
        _psRepoMock.Setup(x => x.PsLineSelectByPs(recId, _ct)).ReturnsAsync(lines);

        var result = await _sut.GetPs(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Penyesuaian Stock retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Header);
        Assert.Equal("PS001", result.Data.Header.PsId);
        Assert.NotNull(result.Data.Lines);
        Assert.Equal(2, result.Data.Lines.Count);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
