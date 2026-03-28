namespace ResearchApps.Service.Tests;

public class ProdServiceTests
{
    private readonly Mock<IProdRepo> _prodRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ProdService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ProdServiceTests()
    {
        _prodRepoMock = new Mock<IProdRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser", UserId = Guid.NewGuid() };
        var loggerMock = new Mock<ILogger<ProdService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new ProdService(
            _prodRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var paged = new PagedList<Prod>(
            new List<Prod>
            {
                new() { RecId = 1, ProdId = "PROD001", ItemName = "Item A", PlanQty = 10 },
                new() { RecId = 2, ProdId = "PROD002", ItemName = "Item B", PlanQty = 20 }
            },
            1,
            10,
            2);

        _prodRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(paged);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Production records retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<PagedListVm<ProdVm>>>(result);
        var data = Assert.IsType<PagedListVm<ProdVm>>(typed.Data);
        Assert.Equal(2, data.TotalCount);
        Assert.Equal(2, data.Items.Count());
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task SelectByIdAsync_WithExistingId_ReturnsRecord()
    {
        const int recId = 1;
        var entity = new Prod { RecId = recId, ProdId = "PROD001", ItemName = "Item A", PlanQty = 10 };

        _prodRepoMock
            .Setup(x => x.SelectByIdAsync(recId, _ct))
            .ReturnsAsync(entity);

        var result = await _sut.SelectByIdAsync(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Production record retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<ProdVm>>(result);
        var data = Assert.IsType<ProdVm>(typed.Data);
        Assert.Equal(recId, data.RecId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task SelectByProdIdAsync_WithExistingId_ReturnsRecord()
    {
        const string prodId = "PROD001";
        var entity = new Prod { RecId = 1, ProdId = prodId, ItemName = "Item A", PlanQty = 10 };

        _prodRepoMock
            .Setup(x => x.SelectByProdIdAsync(prodId, _ct))
            .ReturnsAsync(entity);

        var result = await _sut.SelectByProdIdAsync(prodId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Production record retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<ProdVm>>(result);
        var data = Assert.IsType<ProdVm>(typed.Data);
        Assert.Equal(prodId, data.ProdId);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task InsertAsync_WithValidInput_SetsAuditFieldsAndCommits()
    {
        var prodVm = new ProdVm
        {
            ProdDate = new DateTime(2026, 1, 1),
            CustomerId = 10,
            ItemId = 100,
            PlanQty = 15,
            Notes = "Initial run"
        };

        Prod? capturedEntity = null;
        const int insertedRecId = 88;
        var insertedEntity = new Prod
        {
            RecId = insertedRecId,
            ProdId = "PROD088",
            ProdDate = prodVm.ProdDate,
            CustomerId = prodVm.CustomerId,
            ItemId = prodVm.ItemId,
            PlanQty = prodVm.PlanQty,
            CreatedBy = _userClaimDto.Username,
            ModifiedBy = _userClaimDto.Username,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _prodRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Prod>(), _ct))
            .Callback<Prod, CancellationToken>((p, _) => capturedEntity = p)
            .ReturnsAsync(insertedRecId);

        _prodRepoMock
            .Setup(x => x.SelectByIdAsync(insertedRecId, _ct))
            .ReturnsAsync(insertedEntity);

        var result = await _sut.InsertAsync(prodVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Production record created successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<ProdVm>>(result);
        var data = Assert.IsType<ProdVm>(typed.Data);
        Assert.Equal(insertedRecId, data.RecId);

        Assert.NotNull(capturedEntity);
        Assert.Equal(_userClaimDto.Username, capturedEntity.CreatedBy);
        Assert.Equal(_userClaimDto.Username, capturedEntity.ModifiedBy);
        Assert.NotEqual(default, capturedEntity.CreatedDate);
        Assert.NotEqual(default, capturedEntity.ModifiedDate);

        _prodRepoMock.Verify(x => x.InsertAsync(It.IsAny<Prod>(), _ct), Times.Once);
        _prodRepoMock.Verify(x => x.SelectByIdAsync(insertedRecId, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var prodVm = new ProdVm
        {
            ProdDate = new DateTime(2026, 1, 1),
            CustomerId = 10,
            ItemId = 100,
            PlanQty = 15
        };

        _prodRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Prod>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(prodVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidInput_SetsModifiedFieldsAndCommits()
    {
        var prodVm = new ProdVm
        {
            RecId = 15,
            ProdId = "PROD015",
            ProdDate = new DateTime(2026, 2, 1),
            CustomerId = 10,
            ItemId = 100,
            PlanQty = 20
        };

        Prod? capturedEntity = null;
        var updatedEntity = new Prod
        {
            RecId = prodVm.RecId,
            ProdId = prodVm.ProdId,
            ProdDate = prodVm.ProdDate,
            CustomerId = prodVm.CustomerId,
            ItemId = prodVm.ItemId,
            PlanQty = prodVm.PlanQty,
            ModifiedBy = _userClaimDto.Username,
            ModifiedDate = DateTime.UtcNow
        };

        _prodRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Prod>(), _ct))
            .Callback<Prod, CancellationToken>((p, _) => capturedEntity = p)
            .Returns(Task.CompletedTask);

        _prodRepoMock
            .Setup(x => x.SelectByIdAsync(prodVm.RecId, _ct))
            .ReturnsAsync(updatedEntity);

        var result = await _sut.UpdateAsync(prodVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Production record updated successfully.", result.Message);
        Assert.NotNull(capturedEntity);
        Assert.Equal(_userClaimDto.Username, capturedEntity.ModifiedBy);
        Assert.NotEqual(default, capturedEntity.ModifiedDate);

        _prodRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Prod>(), _ct), Times.Once);
        _prodRepoMock.Verify(x => x.SelectByIdAsync(prodVm.RecId, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var prodVm = new ProdVm
        {
            RecId = 15,
            ProdDate = new DateTime(2026, 2, 1),
            CustomerId = 10,
            ItemId = 100,
            PlanQty = 20
        };

        _prodRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Prod>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.UpdateAsync(prodVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsSuccessAndCommits()
    {
        const int recId = 15;

        _prodRepoMock
            .Setup(x => x.DeleteAsync(recId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(recId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Production record deleted successfully.", result.Message);
        _prodRepoMock.Verify(x => x.DeleteAsync(recId, _ct), Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ProdStatusCboAsync_WithResults_ReturnsMappedStatuses()
    {
        var statuses = new List<ProdStatus>
        {
            new() { ProdStatusId = 1, ProdStatusName = "Open" },
            new() { ProdStatusId = 2, ProdStatusName = "Closed" }
        };

        _prodRepoMock
            .Setup(x => x.ProdStatusCboAsync(_ct))
            .ReturnsAsync(statuses);

        var result = await _sut.ProdStatusCboAsync(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Production statuses retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<ProdStatusVm>>>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<ProdStatusVm>>(typed.Data).ToList();
        Assert.Equal(2, data.Count);
        Assert.Equal("Open", data[0].ProdStatusName);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
