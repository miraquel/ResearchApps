namespace ResearchApps.Service.Tests;

public class PrServiceTests
{
    private readonly Mock<IPrRepo> _prRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly PrService _sut;

    public PrServiceTests()
    {
        _prRepoMock = new Mock<IPrRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<PrService>>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new PrService(
            _prRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task PrSelect_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
        var prs = new PagedList<Pr>(
            new List<Pr>
            {
                new() { RecId = 1, PrId = "PR001", PrName = "PR 1" },
                new() { RecId = 2, PrId = "PR002", PrName = "PR 2" }
            },
            1,
            10,
            2
        );

        _prRepoMock
            .Setup(x => x.PrSelect(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(prs);

        // Act
        var result = await _sut.PrSelect(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PRs retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task PrSelectById_WithValidId_ReturnsPr()
    {
        // Arrange
        var prRecId = 1;
        var cancellationToken = CancellationToken.None;
        var pr = new Pr { RecId = prRecId, PrId = "PR001", PrName = "Test PR" };

        _prRepoMock
            .Setup(x => x.PrSelectById(prRecId, cancellationToken))
            .ReturnsAsync(pr);

        // Act
        var result = await _sut.PrSelectById(prRecId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(prRecId, data.RecId);
    }

    [Fact]
    public async Task PrInsert_WithValidPr_ReturnsInsertedId()
    {
        // Arrange
        var prVm = new PrVm { PrName = "New PR", PrDate = DateTime.Now };
        var cancellationToken = CancellationToken.None;
        var insertedId = 10;

        _prRepoMock
            .Setup(x => x.PrInsert(It.IsAny<Pr>(), cancellationToken))
            .ReturnsAsync(insertedId);

        // Act
        var result = await _sut.PrInsert(prVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertedId, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrInsert_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var prVm = new PrVm { PrName = "New PR" };
        var cancellationToken = CancellationToken.None;
        Pr? capturedPr = null;

        _prRepoMock
            .Setup(x => x.PrInsert(It.IsAny<Pr>(), cancellationToken))
            .Callback<Pr, CancellationToken>((p, _) => capturedPr = p)
            .ReturnsAsync(1);

        // Act
        await _sut.PrInsert(prVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedPr);
        Assert.Equal(_userClaimDto.Username, capturedPr.CreatedBy);
    }

    [Fact]
    public async Task PrUpdate_WithValidPr_CommitsTransaction()
    {
        // Arrange
        var prVm = new PrVm { RecId = 1, PrName = "Updated PR" };
        var cancellationToken = CancellationToken.None;

        _prRepoMock
            .Setup(x => x.PrUpdate(It.IsAny<Pr>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PrUpdate(prVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrUpdate_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var prVm = new PrVm { RecId = 1, PrName = "Updated PR" };
        var cancellationToken = CancellationToken.None;
        Pr? capturedPr = null;

        _prRepoMock
            .Setup(x => x.PrUpdate(It.IsAny<Pr>(), cancellationToken))
            .Callback<Pr, CancellationToken>((p, _) => capturedPr = p)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.PrUpdate(prVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedPr);
        Assert.Equal(_userClaimDto.Username, capturedPr.ModifiedBy);
    }

    [Fact]
    public async Task PrDelete_WithValidId_CommitsTransaction()
    {
        // Arrange
        var prRecId = 1;
        var cancellationToken = CancellationToken.None;

        _prRepoMock
            .Setup(x => x.PrDelete(prRecId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PrDelete(prRecId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrSubmitById_WithValidId_ReturnsSubmittedPr()
    {
        // Arrange
        var prRecId = 1;
        var cancellationToken = CancellationToken.None;
        var submittedPr = new Pr 
        { 
            RecId = prRecId, 
            PrId = "PR001", 
            PrName = "Test PR",
            CurrentApprover = "approver1"
        };

        _prRepoMock
            .Setup(x => x.PrSubmitById(prRecId, _userClaimDto.Username, cancellationToken))
            .ReturnsAsync(submittedPr);

        // Act
        var result = await _sut.PrSubmitById(prRecId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR submitted for approval successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrApproveById_WithValidAction_CommitsTransaction()
    {
        // Arrange
        var action = new PrWorkflowActionVm { RecId = 1, Notes = "Approved" };
        var cancellationToken = CancellationToken.None;

        _prRepoMock
            .Setup(x => x.PrApproveById(action.RecId, action.Notes, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PrApproveById(action, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR approved successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrApproveById_WithNullNotes_UsesEmptyString()
    {
        // Arrange
        var action = new PrWorkflowActionVm { RecId = 1, Notes = null };
        var cancellationToken = CancellationToken.None;
        string? capturedNotes = null;

        _prRepoMock
            .Setup(x => x.PrApproveById(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
            .Callback<int, string, string, CancellationToken>((_, notes, _, _) => capturedNotes = notes)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.PrApproveById(action, cancellationToken);

        // Assert
        Assert.Equal(string.Empty, capturedNotes);
    }

    [Fact]
    public async Task PrRejectById_WithValidAction_CommitsTransaction()
    {
        // Arrange
        var action = new PrWorkflowActionVm { RecId = 1, Notes = "Rejected due to..." };
        var cancellationToken = CancellationToken.None;

        _prRepoMock
            .Setup(x => x.PrRejectById(action.RecId, action.Notes, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PrRejectById(action, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR rejected successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrRecallById_WithValidId_CommitsTransaction()
    {
        // Arrange
        var prRecId = 1;
        var cancellationToken = CancellationToken.None;

        _prRepoMock
            .Setup(x => x.PrRecallById(prRecId, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.PrRecallById(prRecId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("PR recalled successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var prVm = new PrVm { PrName = "New PR" };
        var cancellationToken = CancellationToken.None;

        _prRepoMock
            .Setup(x => x.PrInsert(It.IsAny<Pr>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.PrInsert(prVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task PrSubmitById_PassesCorrectUsernameToRepo()
    {
        // Arrange
        var prRecId = 1;
        var cancellationToken = CancellationToken.None;
        string? capturedUsername = null;

        _prRepoMock
            .Setup(x => x.PrSubmitById(It.IsAny<int>(), It.IsAny<string>(), cancellationToken))
            .Callback<int, string, CancellationToken>((_, username, _) => capturedUsername = username)
            .ReturnsAsync(new Pr { RecId = prRecId });

        // Act
        await _sut.PrSubmitById(prRecId, cancellationToken);

        // Assert
        Assert.Equal(_userClaimDto.Username, capturedUsername);
    }
}



