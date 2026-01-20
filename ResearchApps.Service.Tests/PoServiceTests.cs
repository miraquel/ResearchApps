using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Tests;

public class PoServiceTests
{
    private readonly Mock<IPoRepo> _poRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<PoService>> _loggerMock;
    private readonly PoService _sut;

    public PoServiceTests()
    {
        _poRepoMock = new Mock<IPoRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<PoService>>();

        _sut = new PoService(
            _poRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    #region CRUD Operations Tests

    [Fact]
    public async Task PoSelect_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var poList = new List<Po>
        {
            new() { RecId = 1, PoId = "PO001", SupplierName = "Test Supplier 1" },
            new() { RecId = 2, PoId = "PO002", SupplierName = "Test Supplier 2" }
        };

        _poRepoMock
            .Setup(x => x.PoSelect(It.IsAny<PagedListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedList<Po>(poList, 1, 10, 2));

        // Act
        var result = await _sut.PoSelect(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());
    }

    [Fact]
    public async Task PoSelectById_WithExistingId_ReturnsPo()
    {
        // Arrange
        var recId = 1;
        var po = new Po { RecId = recId, PoId = "PO001", SupplierName = "Test Supplier" };

        _poRepoMock
            .Setup(x => x.PoSelectById(recId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);

        // Act
        var result = await _sut.PoSelectById(recId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("PO001", result.Data.Header.PoId);
    }

    [Fact]
    public async Task PoSelectById_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var recId = 999;

        _poRepoMock
            .Setup(x => x.PoSelectById(recId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Po?)null);

        // Act
        var result = await _sut.PoSelectById(recId, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task PoInsert_WithValidData_ReturnsCreatedAndCommits()
    {
        // Arrange
        var poVm = new PoHeaderVm
        {
            PoDate = DateTime.Now,
            SupplierId = 1
        };

        var insertedPo = new Po
        {
            RecId = 1,
            PoId = "PO001",
            SupplierId = 1,
            CreatedBy = "testuser"
        };

        _poRepoMock
            .Setup(x => x.PoInsert(It.IsAny<Po>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(insertedPo);

        // Act
        var result = await _sut.PoInsert(poVm, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("PO001", result.Data.Header.PoId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoUpdate_WithValidData_ReturnsUpdatedAndCommits()
    {
        // Arrange
        var poVm = new PoHeaderVm
        {
            RecId = 1,
            PoId = "PO001",
            PoDate = DateTime.Now,
            SupplierId = 1
        };

        var updatedPo = new Po
        {
            RecId = 1,
            PoId = "PO001",
            SupplierName = "Updated Supplier",
            ModifiedBy = "testuser"
        };

        _poRepoMock
            .Setup(x => x.PoUpdate(It.IsAny<Po>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedPo);

        // Act
        var result = await _sut.PoUpdate(poVm, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Updated Supplier", result.Data.Header.SupplierName);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoDelete_WithValidId_ReturnsSuccessAndCommits()
    {
        // Arrange
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoDelete(recId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PoDelete(recId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region Workflow Operations Tests

    [Fact]
    public async Task PoSubmitById_WithValidId_ReturnsPoWithCurrentApprover()
    {
        // Arrange
        var recId = 1;
        var submittedPo = new Po
        {
            RecId = recId,
            PoId = "PO001",
            PoStatusId = 4,
            CurrentApprover = "approver1"
        };

        _poRepoMock
            .Setup(x => x.PoSubmitById(recId, _userClaimDto.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submittedPo);

        // Act
        var result = await _sut.PoSubmitById(recId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("approver1", result.Data.Header.CurrentApprover);
        Assert.Equal(4, result.Data.Header.PoStatusId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoRecallById_WithValidId_ReturnsSuccessAndCommits()
    {
        // Arrange
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoRecallById(recId, _userClaimDto.Username, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PoRecallById(recId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoApproveById_WithValidAction_ReturnsSuccessAndCommits()
    {
        // Arrange
        var action = new PoWorkflowActionVm { RecId = 1, Notes = "Approved" };

        _poRepoMock
            .Setup(x => x.PoApproveById(action.RecId, action.Notes, _userClaimDto.Username, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PoApproveById(action, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoRejectById_WithValidAction_ReturnsSuccessAndCommits()
    {
        // Arrange
        var action = new PoWorkflowActionVm { RecId = 1, Notes = "Rejected - insufficient budget" };

        _poRepoMock
            .Setup(x => x.PoRejectById(action.RecId, action.Notes, _userClaimDto.Username, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PoRejectById(action, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PoCloseById_WithValidId_ReturnsSuccessAndCommits()
    {
        // Arrange
        var recId = 1;

        _poRepoMock
            .Setup(x => x.PoCloseById(recId, _userClaimDto.Username, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PoCloseById(recId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region Outstanding Operations Tests

    [Fact]
    public async Task PoOsSelect_WithValidSupplierId_ReturnsOutstandingHeaders()
    {
        // Arrange
        var supplierId = 1;
        var osHeaders = new List<PoHeaderOutstanding>
        {
            new() { RecId = 1, PoId = "PO001", SupplierId = supplierId },
            new() { RecId = 2, PoId = "PO002", SupplierId = supplierId }
        };

        _poRepoMock
            .Setup(x => x.PoOsSelect(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(osHeaders);

        // Act
        var result = await _sut.PoOsSelect(supplierId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task PoOsSelectById_WithValidRecId_ReturnsOutstandingLines()
    {
        // Arrange
        var recId = 1;
        var osLines = new List<PoLineOutstanding>
        {
            new() { PoLineId = 1, PoId = "PO001", ItemId = 1, OutstandingQty = 10 },
            new() { PoLineId = 2, PoId = "PO001", ItemId = 2, OutstandingQty = 5 }
        };

        _poRepoMock
            .Setup(x => x.PoOsSelectById(recId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(osLines);

        // Act
        var result = await _sut.PoOsSelectById(recId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    #endregion
}
