namespace ResearchApps.Service.Tests;

public class UnitServiceTests
{
    private readonly Mock<IUnitRepo> _unitRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<UnitService>> _loggerMock;
    private readonly UnitService _sut;

    public UnitServiceTests()
    {
        _unitRepoMock = new Mock<IUnitRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<UnitService>>();

        _sut = new UnitService(
            _unitRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
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
            .Setup(x => x.UnitSelectAsync(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(units);

        // Act
        var result = await _sut.SelectAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Units retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsUnit()
    {
        // Arrange
        var unitId = 1;
        var cancellationToken = CancellationToken.None;
        var unit = new Unit { UnitId = unitId, UnitName = "PCS" };

        _unitRepoMock
            .Setup(x => x.UnitSelectByIdAsync(unitId, cancellationToken))
            .ReturnsAsync(unit);

        // Act
        var result = await _sut.SelectByIdAsync(unitId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Unit retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task InsertAsync_WithValidUnit_ReturnsInsertedUnit()
    {
        // Arrange
        var unitVm = new UnitVm { UnitName = "BOX" };
        var cancellationToken = CancellationToken.None;
        var insertedUnit = new Unit { UnitId = 3, UnitName = "BOX" };

        _unitRepoMock
            .Setup(x => x.UnitInsertAsync(It.IsAny<Unit>(), cancellationToken))
            .ReturnsAsync(insertedUnit);

        // Act
        var result = await _sut.InsertAsync(unitVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Unit inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var unitVm = new UnitVm { UnitName = "BOX" };
        var cancellationToken = CancellationToken.None;
        Unit? capturedUnit = null;

        _unitRepoMock
            .Setup(x => x.UnitInsertAsync(It.IsAny<Unit>(), cancellationToken))
            .Callback<Unit, CancellationToken>((u, _) => capturedUnit = u)
            .ReturnsAsync(new Unit { UnitId = 1 });

        // Act
        await _sut.InsertAsync(unitVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedUnit);
        Assert.Equal(_userClaimDto.Username, capturedUnit.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidUnit_ReturnsUpdatedUnit()
    {
        // Arrange
        var unitVm = new UnitVm { UnitId = 1, UnitName = "PIECE" };
        var cancellationToken = CancellationToken.None;
        var updatedUnit = new Unit { UnitId = 1, UnitName = "PIECE" };

        _unitRepoMock
            .Setup(x => x.UnitUpdateAsync(It.IsAny<Unit>(), cancellationToken))
            .ReturnsAsync(updatedUnit);

        // Act
        var result = await _sut.UpdateAsync(unitVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Unit updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var unitVm = new UnitVm { UnitId = 1, UnitName = "PIECE" };
        var cancellationToken = CancellationToken.None;
        Unit? capturedUnit = null;

        _unitRepoMock
            .Setup(x => x.UnitUpdateAsync(It.IsAny<Unit>(), cancellationToken))
            .Callback<Unit, CancellationToken>((u, _) => capturedUnit = u)
            .ReturnsAsync(new Unit { UnitId = 1 });

        // Act
        await _sut.UpdateAsync(unitVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedUnit);
        Assert.Equal(_userClaimDto.Username, capturedUnit.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
    {
        // Arrange
        var unitId = 1;
        var cancellationToken = CancellationToken.None;

        _unitRepoMock
            .Setup(x => x.UnitDeleteAsync(unitId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(unitId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Unit deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_WithValidRequest_ReturnsUnitList()
    {
        // Arrange
        var request = new CboRequestVm { Term = "P" };
        var cancellationToken = CancellationToken.None;
        var units = new List<Unit>
        {
            new() { UnitId = 1, UnitName = "PCS" },
            new() { UnitId = 2, UnitName = "PACK" }
        };

        _unitRepoMock
            .Setup(x => x.UnitCboAsync(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(units);

        // Act
        var result = await _sut.CboAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var unitVm = new UnitVm { UnitName = "BOX" };
        var cancellationToken = CancellationToken.None;

        _unitRepoMock
            .Setup(x => x.UnitInsertAsync(It.IsAny<Unit>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(unitVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}






