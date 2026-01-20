using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Tests;

public class PoLineServiceTests
{
    private readonly Mock<IPoLineRepo> _poLineRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<PoLineService>> _loggerMock;
    private readonly PoLineService _sut;

    public PoLineServiceTests()
    {
        _poLineRepoMock = new Mock<IPoLineRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<PoLineService>>();

        _sut = new PoLineService(
            _poLineRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    [Fact]
    public async Task PoLineSelectById_WithExistingId_ReturnsPoLine()
    {
        // Arrange
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
            .Setup(x => x.PoLineSelectById(poLineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(poLine);

        // Act
        var result = await _sut.PoLineSelectById(poLineId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("PO001", result.Data.PoId);
    }

    [Fact]
    public async Task PoLineSelectById_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var poLineId = 999;

        _poLineRepoMock
            .Setup(x => x.PoLineSelectById(poLineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PoLine?)null);

        // Act
        var result = await _sut.PoLineSelectById(poLineId, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task PoLineSelectByPo_WithValidPoId_ReturnsLines()
    {
        // Arrange
        const int poRecId = 1;
        var poLines = new List<PoLine>
        {
            new() { PoLineId = 1, RecId = poRecId, ItemId = 1 },
            new() { PoLineId = 2, RecId = poRecId, ItemId = 2 }
        };

        _poLineRepoMock
            .Setup(x => x.PoLineSelectByPo(poRecId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(poLines);

        // Act
        var result = await _sut.PoLineSelectByPo(poRecId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task PoLineInsert_WithValidData_ReturnsPoIdAndCommits()
    {
        // Arrange
        var poLineVm = new PoLineVm
        {
            PoId = "PO001",
            ItemId = 1,
            Qty = 10,
            Price = 100,
            UnitId = 1
        };

        _poLineRepoMock
            .Setup(x => x.PoLineInsert(It.IsAny<PoLine>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("PO001");

        // Act
        var result = await _sut.PoLineInsert(poLineVm, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("PO001", result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoLineDelete_WithValidId_ReturnsPoIdAndCommits()
    {
        // Arrange
        var poLineId = 1;
        var poLine = new PoLine { PoLineId = poLineId, PoId = "PO001" };

        _poLineRepoMock
            .Setup(x => x.PoLineSelectById(poLineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(poLine);

        _poLineRepoMock
            .Setup(x => x.PoLineDelete(poLineId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PoLineDelete(poLineId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PO001", result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoLineInsert_SetsAuditFields()
    {
        // Arrange
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
            .Setup(x => x.PoLineInsert(It.IsAny<PoLine>(), It.IsAny<CancellationToken>()))
            .Callback<PoLine, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync("PO001");

        // Act
        await _sut.PoLineInsert(poLineVm, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedEntity);
        Assert.Equal("testuser", capturedEntity.CreatedBy);
        Assert.Equal("testuser", capturedEntity.ModifiedBy);
    }
}
