namespace ResearchApps.Service.Tests;

public class SupplierServiceTests
{
    private readonly Mock<ISupplierRepo> _supplierRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly SupplierService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public SupplierServiceTests()
    {
        _supplierRepoMock = new Mock<ISupplierRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<SupplierService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new SupplierService(
            _supplierRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SupplierSelect_NoArgs_ReturnsSuccess()
    {
        var suppliers = new List<Domain.Supplier>
        {
            new() { SupplierId = 1, SupplierName = "Supplier A" },
            new() { SupplierId = 2, SupplierName = "Supplier B" }
        };

        _supplierRepoMock
            .Setup(x => x.SupplierSelect(_ct))
            .ReturnsAsync(suppliers);

        var result = await _sut.SupplierSelect(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Suppliers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SupplierSelect_WithPagedRequest_ReturnsSuccess()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var suppliers = new PagedList<Domain.Supplier>(
            new List<Domain.Supplier>
            {
                new() { SupplierId = 1, SupplierName = "Supplier A" },
                new() { SupplierId = 2, SupplierName = "Supplier B" }
            },
            1,
            10,
            2
        );

        _supplierRepoMock
            .Setup(x => x.SupplierSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(suppliers);

        var result = await _sut.SupplierSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Suppliers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SupplierSelectById_WhenFound_ReturnsSuccess()
    {
        var supplierId = 1;
        var supplier = new Domain.Supplier { SupplierId = supplierId, SupplierName = "Supplier A" };

        _supplierRepoMock
            .Setup(x => x.SupplierSelectById(supplierId, _ct))
            .ReturnsAsync(supplier);

        var result = await _sut.SupplierSelectById(supplierId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Supplier retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SupplierSelectById_WhenNotFound_ReturnsFailure404()
    {
        var supplierId = 99;

        _supplierRepoMock
            .Setup(x => x.SupplierSelectById(supplierId, _ct))
            .ReturnsAsync((Domain.Supplier?)null);

        var result = await _sut.SupplierSelectById(supplierId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(result.Errors);
        Assert.Contains("Supplier not found.", result.Errors);
    }

    [Fact]
    public async Task SupplierInsert_WithValidSupplier_ReturnsSuccess201()
    {
        var supplierVm = new SupplierVm { SupplierName = "New Supplier" };
        var inserted = new Domain.Supplier { SupplierId = 5, SupplierName = "New Supplier" };

        _supplierRepoMock
            .Setup(x => x.SupplierInsert(It.IsAny<Domain.Supplier>(), _ct))
            .ReturnsAsync(inserted);

        var result = await _sut.SupplierInsert(supplierVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(201, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SupplierInsert_SetsCreatedByFromUserClaim()
    {
        var supplierVm = new SupplierVm { SupplierName = "New Supplier" };
        Domain.Supplier? captured = null;

        _supplierRepoMock
            .Setup(x => x.SupplierInsert(It.IsAny<Domain.Supplier>(), _ct))
            .Callback<Domain.Supplier, CancellationToken>((s, _) => captured = s)
            .ReturnsAsync(new Domain.Supplier { SupplierId = 1 });

        await _sut.SupplierInsert(supplierVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.CreatedBy);
    }

    [Fact]
    public async Task SupplierInsert_WhenRepoThrowsRepoException_ReturnsFailure400WithNoCommit()
    {
        var supplierVm = new SupplierVm { SupplierName = "Duplicate Supplier" };

        _supplierRepoMock
            .Setup(x => x.SupplierInsert(It.IsAny<Domain.Supplier>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException<Domain.Supplier>("Duplicate name", new Domain.Supplier()));

        var result = await _sut.SupplierInsert(supplierVm, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task SupplierUpdate_WithValidSupplier_ReturnsSuccess()
    {
        var supplierVm = new SupplierVm { SupplierId = 1, SupplierName = "Updated Supplier" };
        var updated = new Domain.Supplier { SupplierId = 1, SupplierName = "Updated Supplier" };

        _supplierRepoMock
            .Setup(x => x.SupplierUpdate(It.IsAny<Domain.Supplier>(), _ct))
            .ReturnsAsync(updated);

        var result = await _sut.SupplierUpdate(supplierVm, _ct);

        Assert.True(result.IsSuccess);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SupplierUpdate_SetsModifiedByFromUserClaim()
    {
        var supplierVm = new SupplierVm { SupplierId = 1, SupplierName = "Updated Supplier" };
        Domain.Supplier? captured = null;

        _supplierRepoMock
            .Setup(x => x.SupplierUpdate(It.IsAny<Domain.Supplier>(), _ct))
            .Callback<Domain.Supplier, CancellationToken>((s, _) => captured = s)
            .ReturnsAsync(new Domain.Supplier { SupplierId = 1 });

        await _sut.SupplierUpdate(supplierVm, _ct);

        Assert.NotNull(captured);
        Assert.Equal(_userClaimDto.Username, captured.ModifiedBy);
    }

    [Fact]
    public async Task SupplierUpdate_WhenRepoThrowsRepoException_ReturnsFailure400WithNoCommit()
    {
        var supplierVm = new SupplierVm { SupplierId = 1, SupplierName = "Duplicate Supplier" };

        _supplierRepoMock
            .Setup(x => x.SupplierUpdate(It.IsAny<Domain.Supplier>(), _ct))
            .ThrowsAsync(new ResearchApps.Common.Exceptions.RepoException<Domain.Supplier>("Duplicate name", new Domain.Supplier()));

        var result = await _sut.SupplierUpdate(supplierVm, _ct);

        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task SupplierDelete_WithValidId_CommitsAndReturnsSuccess()
    {
        var supplierId = 1;

        _supplierRepoMock
            .Setup(x => x.SupplierDelete(supplierId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.SupplierDelete(supplierId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Supplier deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task SupplierCbo_ReturnsSuccess()
    {
        var suppliers = new List<Domain.Supplier>
        {
            new() { SupplierId = 1, SupplierName = "Supplier A" },
            new() { SupplierId = 2, SupplierName = "Supplier B" }
        };

        _supplierRepoMock
            .Setup(x => x.SupplierCbo(_ct))
            .ReturnsAsync(suppliers);

        var result = await _sut.SupplierCbo(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Suppliers retrieved successfully.", result.Message);
    }
}
