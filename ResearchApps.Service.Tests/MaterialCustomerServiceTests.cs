namespace ResearchApps.Service.Tests;

public class MaterialCustomerServiceTests
{
    private readonly Mock<IMaterialCustomerRepo> _repoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly MaterialCustomerService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public MaterialCustomerServiceTests()
    {
        _repoMock = new Mock<IMaterialCustomerRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<MaterialCustomerService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new MaterialCustomerService(
            _repoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task McSelect_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var paged = new PagedList<MaterialCustomerHeader>(
            new List<MaterialCustomerHeader>
            {
                new() { RecId = 1, McId = "MC001" },
                new() { RecId = 2, McId = "MC002" }
            }, 1, 10, 2);

        _repoMock
            .Setup(x => x.McSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(paged);

        var result = await _sut.McSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customers retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<PagedListVm<MaterialCustomerHeaderVm>>>(result);
        Assert.Equal(2, typed.Data!.TotalCount);
        Assert.Equal(2, typed.Data.Items.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task McSelectById_WithValidId_ReturnsMc()
    {
        var entity = new MaterialCustomerHeader { RecId = 1, McId = "MC001" };

        _repoMock
            .Setup(x => x.McSelectById(1, _ct))
            .ReturnsAsync(entity);

        var result = await _sut.McSelectById(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customer retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<MaterialCustomerHeaderVm>>(result);
        Assert.Equal(1, typed.Data!.RecId);
        Assert.Equal("MC001", typed.Data.McId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task McInsert_WithNoLines_CommitsAndReturns201()
    {
        var vm = new MaterialCustomerVm
        {
            Header = new MaterialCustomerHeaderVm(),
            Lines = new List<MaterialCustomerLineVm>()
        };

        _repoMock
            .Setup(x => x.McInsert(It.IsAny<MaterialCustomerHeader>(), _ct))
            .ReturnsAsync((1, "MC001"));

        var result = await _sut.McInsert(vm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Material Customer created successfully.", result.Message);
        Assert.Equal((1, "MC001"), result.Data);
        _repoMock.Verify(x => x.McInsert(It.IsAny<MaterialCustomerHeader>(), _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task McInsert_SetsCreatedByFromUserClaim()
    {
        var vm = new MaterialCustomerVm
        {
            Header = new MaterialCustomerHeaderVm(),
            Lines = new List<MaterialCustomerLineVm>()
        };

        MaterialCustomerHeader? captured = null;

        _repoMock
            .Setup(x => x.McInsert(It.IsAny<MaterialCustomerHeader>(), _ct))
            .Callback<MaterialCustomerHeader, CancellationToken>((e, _) => captured = e)
            .ReturnsAsync((1, "MC001"));

        await _sut.McInsert(vm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task McUpdate_WithValidMc_CommitsTransaction()
    {
        var headerVm = new MaterialCustomerHeaderVm { RecId = 1, McId = "MC001" };

        _repoMock
            .Setup(x => x.McUpdate(It.IsAny<MaterialCustomerHeader>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.McUpdate(headerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customer updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task McUpdate_SetsModifiedByFromUserClaim()
    {
        var headerVm = new MaterialCustomerHeaderVm { RecId = 1, McId = "MC001" };
        MaterialCustomerHeader? captured = null;

        _repoMock
            .Setup(x => x.McUpdate(It.IsAny<MaterialCustomerHeader>(), _ct))
            .Callback<MaterialCustomerHeader, CancellationToken>((e, _) => captured = e)
            .Returns(Task.CompletedTask);

        await _sut.McUpdate(headerVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.ModifiedBy);
    }

    [Fact]
    public async Task McDelete_WithValidId_CommitsTransaction()
    {
        _repoMock
            .Setup(x => x.McDelete(1, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.McDelete(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customer deleted successfully.", result.Message);
        _repoMock.Verify(x => x.McDelete(1, _userClaimDto.Username, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task McLineSelectByMc_WithValidId_ReturnsLines()
    {
        var lines = new List<MaterialCustomerLine>
        {
            new() { McLineId = 1, RecId = 1 },
            new() { McLineId = 2, RecId = 1 }
        };

        _repoMock
            .Setup(x => x.McLineSelectByMc(1, _ct))
            .ReturnsAsync(lines);

        var result = await _sut.McLineSelectByMc(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customer lines retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<MaterialCustomerLineVm>>>(result);
        Assert.Equal(2, typed.Data!.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task McLineSelectById_WithValidId_ReturnsLine()
    {
        var line = new MaterialCustomerLine { McLineId = 1, RecId = 1 };

        _repoMock
            .Setup(x => x.McLineSelectById(1, _ct))
            .ReturnsAsync(line);

        var result = await _sut.McLineSelectById(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customer line retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<MaterialCustomerLineVm>>(result);
        Assert.Equal(1, typed.Data!.McLineId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task McLineSelectById_WithInvalidId_ReturnsNotFound()
    {
        _repoMock
            .Setup(x => x.McLineSelectById(999, _ct))
            .ReturnsAsync((MaterialCustomerLine?)null);

        var result = await _sut.McLineSelectById(999, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task McLineInsert_WithValidLine_CommitsAndCreated()
    {
        var lineVm = new MaterialCustomerLineVm { RecId = 1 };

        _repoMock
            .Setup(x => x.McLineInsert(It.IsAny<MaterialCustomerLine>(), _ct))
            .ReturnsAsync("MCL001");

        var result = await _sut.McLineInsert(lineVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Material Customer line created successfully.", result.Message);
        Assert.Equal("MCL001", result.Data);
        _repoMock.Verify(x => x.McLineInsert(It.IsAny<MaterialCustomerLine>(), _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task McLineDelete_WithValidId_CommitsTransaction()
    {
        _repoMock
            .Setup(x => x.McLineDelete(1, _userClaimDto.Username, _ct))
            .ReturnsAsync(string.Empty);

        var result = await _sut.McLineDelete(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customer line deleted successfully.", result.Message);
        _repoMock.Verify(x => x.McLineDelete(1, _userClaimDto.Username, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetMaterialCustomer_WithValidId_ReturnsCompositeVm()
    {
        var header = new MaterialCustomerHeader { RecId = 1, McId = "MC001" };
        var lines = new List<MaterialCustomerLine>
        {
            new() { McLineId = 1, RecId = 1 }
        };

        _repoMock.Setup(x => x.McSelectById(1, _ct)).ReturnsAsync(header);
        _repoMock.Setup(x => x.McLineSelectByMc(1, _ct)).ReturnsAsync(lines);

        var result = await _sut.GetMaterialCustomer(1, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Material Customer retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<MaterialCustomerVm>>(result);
        Assert.NotNull(typed.Data!.Header);
        Assert.NotNull(typed.Data.Lines);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
