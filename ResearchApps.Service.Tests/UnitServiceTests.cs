namespace ResearchApps.Service.Tests;

public class UnitServiceTests
{
    private readonly Mock<IUnitRepo> _unitRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly UnitService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public UnitServiceTests()
    {
        _unitRepoMock = new Mock<IUnitRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<UnitService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new UnitService(
            _unitRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var units = new PagedList<Unit>(
            new List<Unit>
            {
                new() { UnitId = 1, UnitName = "PCS" },
                new() { UnitId = 2, UnitName = "KG" }
            },
            1,
            10,
            2
        );

        _unitRepoMock
            .Setup(x => x.UnitSelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(units);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Units retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsUnit()
    {
        var unitId = 1;
        var unit = new Unit { UnitId = unitId, UnitName = "PCS" };

        _unitRepoMock
            .Setup(x => x.UnitSelectByIdAsync(unitId, _ct))
            .ReturnsAsync(unit);

        var result = await _sut.SelectByIdAsync(unitId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Unit retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidUnit_ReturnsInsertedUnit()
    {
        var unitVm = new UnitVm { UnitName = "BOX" };
        var insertedUnit = new Unit { UnitId = 3, UnitName = "BOX" };

        _unitRepoMock
            .Setup(x => x.UnitInsertAsync(It.IsAny<Unit>(), _ct))
            .ReturnsAsync(insertedUnit);

        var result = await _sut.InsertAsync(unitVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Unit inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var unitVm = new UnitVm { UnitName = "BOX" };
        Unit? capturedUnit = null;

        _unitRepoMock
            .Setup(x => x.UnitInsertAsync(It.IsAny<Unit>(), _ct))
            .Callback<Unit, CancellationToken>((u, _) => capturedUnit = u)
            .ReturnsAsync(new Unit { UnitId = 1 });

        await _sut.InsertAsync(unitVm, _ct);

        Assert.NotNull(capturedUnit);
        Assert.Equal(_userClaimDto.Username, capturedUnit.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidUnit_ReturnsUpdatedUnit()
    {
        var unitVm = new UnitVm { UnitId = 1, UnitName = "PIECE" };
        var updatedUnit = new Unit { UnitId = 1, UnitName = "PIECE" };

        _unitRepoMock
            .Setup(x => x.UnitUpdateAsync(It.IsAny<Unit>(), _ct))
            .ReturnsAsync(updatedUnit);

        var result = await _sut.UpdateAsync(unitVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Unit updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var unitVm = new UnitVm { UnitId = 1, UnitName = "PIECE" };
        Unit? capturedUnit = null;

        _unitRepoMock
            .Setup(x => x.UnitUpdateAsync(It.IsAny<Unit>(), _ct))
            .Callback<Unit, CancellationToken>((u, _) => capturedUnit = u)
            .ReturnsAsync(new Unit { UnitId = 1 });

        await _sut.UpdateAsync(unitVm, _ct);

        Assert.NotNull(capturedUnit);
        Assert.Equal(_userClaimDto.Username, capturedUnit.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        var unitId = 1;

        _unitRepoMock
            .Setup(x => x.UnitDeleteAsync(unitId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(unitId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Unit deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsUnitList()
    {
        var request = new CboRequestVm { Term = "P" };
        var units = new List<Unit>
        {
            new() { UnitId = 1, UnitName = "PCS" },
            new() { UnitId = 2, UnitName = "PACK" }
        };

        _unitRepoMock
            .Setup(x => x.UnitCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(units);

        var result = await _sut.CboAsync(request, _ct);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var unitVm = new UnitVm { UnitName = "BOX" };

        _unitRepoMock
            .Setup(x => x.UnitInsertAsync(It.IsAny<Unit>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(unitVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
