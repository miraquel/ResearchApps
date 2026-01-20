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

/// <summary>
/// Unit tests for SalesInvoiceService covering Sales Invoice operations
/// </summary>
public class SalesInvoiceServiceTests
{
    private readonly Mock<ISalesInvoiceRepo> _siRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly SalesInvoiceService _sut;

    public SalesInvoiceServiceTests()
    {
        _siRepoMock = new Mock<ISalesInvoiceRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<SalesInvoiceService>>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new SalesInvoiceService(
            _siRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    #region SI Header Tests

    [Fact]
    public async Task SiSelect_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
        var sis = new PagedList<SalesInvoiceHeader>(
            new List<SalesInvoiceHeader>
            {
                new() { RecId = 1, SiId = "FNAINV24001" },
                new() { RecId = 2, SiId = "FNAINV24002" }
            },
            1,
            10,
            2
        );

        _siRepoMock
            .Setup(x => x.SiSelect(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(sis);

        // Act
        var result = await _sut.SiSelect(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoices retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalCount);
    }

    [Fact]
    public async Task SiSelectById_WithValidId_ReturnsSi()
    {
        // Arrange
        var recId = 1;
        var cancellationToken = CancellationToken.None;
        var si = new SalesInvoiceHeader { RecId = recId, SiId = "FNAINV24001", CustomerId = 1 };

        _siRepoMock
            .Setup(x => x.SiSelectById(recId, cancellationToken))
            .ReturnsAsync(si);

        // Act
        var result = await _sut.SiSelectById(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("FNAINV24001", result.Data.SiId);
    }

    [Fact]
    public async Task SiInsert_WithValidSi_ReturnsInsertedIdAndSiId()
    {
        // Arrange
        var siVm = new SalesInvoiceVm
        {
            Header = new SalesInvoiceHeaderVm { CustomerId = 1, SiDate = DateTime.Now },
            Lines = new List<SalesInvoiceLineVm>
            {
                new() { DoLineId = 1, DoId = "DO001", ItemId = 1, Qty = 10, Price = 1000 }
            }
        };
        var cancellationToken = CancellationToken.None;
        var insertResult = (RecId: 10, SiId: "FNAINV24010");

        _siRepoMock
            .Setup(x => x.SiInsert(It.IsAny<SalesInvoiceHeader>(), cancellationToken))
            .ReturnsAsync(insertResult);

        _siRepoMock
            .Setup(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), cancellationToken))
            .ReturnsAsync("1:::FNAINV24010");

        // Act
        var result = await _sut.SiInsert(siVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertResult, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SiInsert_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var siVm = new SalesInvoiceVm
        {
            Header = new SalesInvoiceHeaderVm { CustomerId = 1 }
        };
        var cancellationToken = CancellationToken.None;
        SalesInvoiceHeader? capturedSi = null;

        _siRepoMock
            .Setup(x => x.SiInsert(It.IsAny<SalesInvoiceHeader>(), cancellationToken))
            .Callback<SalesInvoiceHeader, CancellationToken>((s, _) => capturedSi = s)
            .ReturnsAsync((1, "FNAINV24001"));

        // Act
        await _sut.SiInsert(siVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedSi);
        Assert.Equal(_userClaimDto.Username, capturedSi.CreatedBy);
    }

    [Fact]
    public async Task SiInsert_WithLines_InsertsAllLines()
    {
        // Arrange
        var siVm = new SalesInvoiceVm
        {
            Header = new SalesInvoiceHeaderVm { CustomerId = 1, SiDate = DateTime.Now },
            Lines = new List<SalesInvoiceLineVm>
            {
                new() { DoLineId = 1, DoId = "DO001", ItemId = 1, Qty = 10, Price = 1000 },
                new() { DoLineId = 2, DoId = "DO001", ItemId = 2, Qty = 5, Price = 2000 },
                new() { DoLineId = 3, DoId = "DO002", ItemId = 3, Qty = 3, Price = 3000 }
            }
        };
        var cancellationToken = CancellationToken.None;

        _siRepoMock
            .Setup(x => x.SiInsert(It.IsAny<SalesInvoiceHeader>(), cancellationToken))
            .ReturnsAsync((1, "FNAINV24001"));

        _siRepoMock
            .Setup(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), cancellationToken))
            .ReturnsAsync("1:::FNAINV24001");

        // Act
        await _sut.SiInsert(siVm, cancellationToken);

        // Assert
        _siRepoMock.Verify(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), cancellationToken), Times.Exactly(3));
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SiUpdate_WithValidSi_CommitsTransaction()
    {
        // Arrange
        var siVm = new SalesInvoiceHeaderVm { RecId = 1, SiId = "FNAINV24001", CustomerId = 1 };
        var cancellationToken = CancellationToken.None;

        _siRepoMock
            .Setup(x => x.SiUpdate(It.IsAny<SalesInvoiceHeader>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.SiUpdate(siVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SiUpdate_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var siVm = new SalesInvoiceHeaderVm { RecId = 1, SiId = "FNAINV24001", CustomerId = 1 };
        var cancellationToken = CancellationToken.None;
        SalesInvoiceHeader? capturedSi = null;

        _siRepoMock
            .Setup(x => x.SiUpdate(It.IsAny<SalesInvoiceHeader>(), cancellationToken))
            .Callback<SalesInvoiceHeader, CancellationToken>((s, _) => capturedSi = s)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SiUpdate(siVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedSi);
        Assert.Equal(_userClaimDto.Username, capturedSi.ModifiedBy);
    }

    [Fact]
    public async Task SiDelete_WithValidId_CommitsTransaction()
    {
        // Arrange
        var recId = 1;
        var cancellationToken = CancellationToken.None;

        _siRepoMock
            .Setup(x => x.SiDelete(recId, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.SiDelete(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region SI Line Tests

    [Fact]
    public async Task SiLineSelectBySi_WithValidSiRecId_ReturnsLines()
    {
        // Arrange
        var siRecId = 1;
        var cancellationToken = CancellationToken.None;
        var lines = new List<SalesInvoiceLine>
        {
            new() { SiLineId = 1, SiId = "FNAINV24001", ItemId = 1, Qty = 10, Price = 1000 },
            new() { SiLineId = 2, SiId = "FNAINV24001", ItemId = 2, Qty = 5, Price = 2000 }
        };

        _siRepoMock
            .Setup(x => x.SiLineSelectBySi(siRecId, cancellationToken))
            .ReturnsAsync(lines);

        // Act
        var result = await _sut.SiLineSelectBySi(siRecId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice lines retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task SiLineSelectById_WithValidId_ReturnsLine()
    {
        // Arrange
        var siLineId = 1;
        var cancellationToken = CancellationToken.None;
        var line = new SalesInvoiceLine { SiLineId = siLineId, SiId = "FNAINV24001", ItemId = 1 };

        _siRepoMock
            .Setup(x => x.SiLineSelectById(siLineId, cancellationToken))
            .ReturnsAsync(line);

        // Act
        var result = await _sut.SiLineSelectById(siLineId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice line retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task SiLineSelectById_WithInvalidId_ReturnsFailure()
    {
        // Arrange
        var siLineId = 999;
        var cancellationToken = CancellationToken.None;

        _siRepoMock
            .Setup(x => x.SiLineSelectById(siLineId, cancellationToken))
            .ReturnsAsync((SalesInvoiceLine?)null);

        // Act
        var result = await _sut.SiLineSelectById(siLineId, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("not found"));
    }

    [Fact]
    public async Task SiLineInsert_WithValidLine_CommitsTransaction()
    {
        // Arrange
        var lineVm = new SalesInvoiceLineVm
        {
            SiRecId = 1,
            DoLineId = 1,
            DoId = "DO001",
            ItemId = 1,
            Qty = 10,
            Price = 1000
        };
        var cancellationToken = CancellationToken.None;

        _siRepoMock
            .Setup(x => x.SiLineInsert(It.IsAny<SalesInvoiceLine>(), cancellationToken))
            .ReturnsAsync("1:::FNAINV24001");

        // Act
        var result = await _sut.SiLineInsert(lineVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice line inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region Composite Tests

    [Fact]
    public async Task GetSalesInvoice_WithValidId_ReturnsHeaderAndLines()
    {
        // Arrange
        var recId = 1;
        var cancellationToken = CancellationToken.None;
        var header = new SalesInvoiceHeader
        {
            RecId = recId,
            SiId = "FNAINV24001",
            CustomerId = 1,
            CustomerName = "Test Customer",
            Amount = 15000
        };
        var lines = new List<SalesInvoiceLine>
        {
            new() { SiLineId = 1, SiId = "FNAINV24001", ItemId = 1, Qty = 10, Price = 1000 },
            new() { SiLineId = 2, SiId = "FNAINV24001", ItemId = 2, Qty = 5, Price = 1000 }
        };

        _siRepoMock
            .Setup(x => x.SiSelectById(recId, cancellationToken))
            .ReturnsAsync(header);

        _siRepoMock
            .Setup(x => x.SiLineSelectBySi(recId, cancellationToken))
            .ReturnsAsync(lines);

        // Act
        var result = await _sut.GetSalesInvoice(recId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Sales Invoice retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("FNAINV24001", result.Data.Header.SiId);
        Assert.Equal(2, result.Data.Lines.Count());
    }

    #endregion
}
