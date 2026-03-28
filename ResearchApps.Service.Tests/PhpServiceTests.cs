namespace ResearchApps.Service.Tests;

public class PhpServiceTests
{
    private readonly Mock<IPhpRepo> _repoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly PhpService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public PhpServiceTests()
    {
        _repoMock = new Mock<IPhpRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<PhpService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new PhpService(
            _repoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task PhpSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var paged = new PagedList<PhpHeader>(
            new List<PhpHeader>
            {
                new() { RecId = 1, PhpId = "PHP001" },
                new() { RecId = 2, PhpId = "PHP002" }
            }, 1, 10, 2);

        _repoMock
            .Setup(x => x.PhpSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(paged);

        var result = await _sut.PhpSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php records retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<PagedListVm<PhpHeaderVm>>>(result);
        Assert.Equal(2, typed.Data!.TotalCount);
        Assert.Equal(2, typed.Data.Items.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PhpSelectById_WithValidId_ReturnsPhp()
    {
        var entity = new PhpHeader { RecId = 1, PhpId = "PHP001" };

        _repoMock
            .Setup(x => x.PhpSelectById(1, _ct))
            .ReturnsAsync(entity);

        var result = await _sut.PhpSelectById(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<PhpHeaderVm>>(result);
        Assert.Equal(1, typed.Data!.RecId);
        Assert.Equal("PHP001", typed.Data.PhpId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PhpInsert_WithValidPhp_CommitsAndReturns201()
    {
        var headerVm = new PhpHeaderVm();

        _repoMock
            .Setup(x => x.PhpInsert(It.IsAny<PhpHeader>(), _ct))
            .ReturnsAsync((1, "PHP001"));

        var result = await _sut.PhpInsert(headerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Php created successfully.", result.Message);
        Assert.Equal((1, "PHP001"), result.Data);
        _repoMock.Verify(x => x.PhpInsert(It.IsAny<PhpHeader>(), _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PhpInsert_SetsCreatedByFromUserClaim()
    {
        var headerVm = new PhpHeaderVm();
        PhpHeader? captured = null;

        _repoMock
            .Setup(x => x.PhpInsert(It.IsAny<PhpHeader>(), _ct))
            .Callback<PhpHeader, CancellationToken>((e, _) => captured = e)
            .ReturnsAsync((1, "PHP001"));

        await _sut.PhpInsert(headerVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task PhpUpdate_WithValidPhp_CommitsTransaction()
    {
        var headerVm = new PhpHeaderVm { RecId = 1, PhpId = "PHP001" };

        _repoMock
            .Setup(x => x.PhpUpdate(It.IsAny<PhpHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PhpUpdate(headerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PhpUpdate_SetsModifiedByFromUserClaim()
    {
        var headerVm = new PhpHeaderVm { RecId = 1, PhpId = "PHP001" };
        PhpHeader? captured = null;

        _repoMock
            .Setup(x => x.PhpUpdate(It.IsAny<PhpHeader>(), _ct))
            .Callback<PhpHeader, CancellationToken>((e, _) => captured = e)
            .Returns(Task.CompletedTask);

        await _sut.PhpUpdate(headerVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.ModifiedBy);
    }

    [Fact]
    public async Task PhpDelete_WithValidId_CommitsTransaction()
    {
        _repoMock
            .Setup(x => x.PhpDelete(1, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.PhpDelete(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php deleted successfully.", result.Message);
        _repoMock.Verify(x => x.PhpDelete(1, _userClaimDto.Username, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PhpLineSelectByPhp_WithValidId_ReturnsLines()
    {
        var lines = new List<PhpLine>
        {
            new() { PhpLineId = 1, PhpRecId = 1 },
            new() { PhpLineId = 2, PhpRecId = 1 }
        };

        _repoMock
            .Setup(x => x.PhpLineSelectByPhp(1, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.PhpLineSelectByPhp(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php lines retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<PhpLineVm>>>(result);
        Assert.Equal(2, typed.Data!.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PhpLineSelectById_WithValidId_ReturnsLine()
    {
        var line = new PhpLine { PhpLineId = 1, PhpRecId = 1 };

        _repoMock
            .Setup(x => x.PhpLineSelectById(1, _ct))
            .ReturnsAsync(line);

        var result = await _sut.PhpLineSelectById(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php line retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<PhpLineVm>>(result);
        Assert.Equal(1, typed.Data!.PhpLineId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PhpLineSelectById_WithInvalidId_ReturnsNotFound()
    {
        _repoMock
            .Setup(x => x.PhpLineSelectById(999, _ct))
            .ReturnsAsync((PhpLine?)null);

        var result = await _sut.PhpLineSelectById(999, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PhpLineInsert_WithValidLine_CommitsAndCreated()
    {
        var lineVm = new PhpLineVm { PhpRecId = 1 };

        _repoMock
            .Setup(x => x.PhpLineInsert(It.IsAny<PhpLine>(), _ct))
            .ReturnsAsync("PHPL001");

        var result = await _sut.PhpLineInsert(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Php line created successfully.", result.Message);
        _repoMock.Verify(x => x.PhpLineInsert(It.IsAny<PhpLine>(), _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }


    [Fact]
    public async Task PhpLineUpdate_WithValidLine_CommitsTransaction()
    {
        var lineVm = new PhpLineVm { PhpLineId = 1, PhpRecId = 1 };

        _repoMock
            .Setup(x => x.PhpLineUpdate(It.IsAny<PhpLine>(), _ct))
            .ReturnsAsync(string.Empty);

        var result = await _sut.PhpLineUpdate(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php line updated successfully.", result.Message);
        _repoMock.Verify(x => x.PhpLineUpdate(It.IsAny<PhpLine>(), _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PhpLineDelete_WithValidId_CommitsTransaction()
    {
        _repoMock
            .Setup(x => x.PhpLineDelete(1, _userClaimDto.Username, _ct))
            .ReturnsAsync(string.Empty);

        var result = await _sut.PhpLineDelete(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php line deleted successfully.", result.Message);
        _repoMock.Verify(x => x.PhpLineDelete(1, _userClaimDto.Username, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetPhp_WithValidId_ReturnsCompositeVm()
    {
        var header = new PhpHeader { RecId = 1, PhpId = "PHP001" };
        var lines = new List<PhpLine>
        {
            new() { PhpLineId = 1, PhpRecId = 1 }
        };

        _repoMock.Setup(x => x.PhpSelectById(1, _ct)).ReturnsAsync(header);
        _repoMock.Setup(x => x.PhpLineSelectByPhp(1, _ct)).ReturnsAsync(lines);

        var result = await _sut.GetPhp(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Php retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<PhpVm>>(result);
        Assert.NotNull(typed.Data!.Header);
        Assert.NotNull(typed.Data.Lines);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
