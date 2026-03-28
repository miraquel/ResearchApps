namespace ResearchApps.Service.Tests;

public class WfFormServiceTests
{
    private readonly Mock<IWfFormRepo> _wfFormRepoMock;
    private readonly Mock<IWfRepo> _wfRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly WfFormService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public WfFormServiceTests()
    {
        _wfFormRepoMock = new Mock<IWfFormRepo>();
        _wfRepoMock = new Mock<IWfRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<WfFormService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new WfFormService(
            _wfFormRepoMock.Object,
            _wfRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task WfFormSelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var wfForms = new PagedList<WfForm>(
            new List<WfForm>
            {
                new() { WfFormId = 1, FormName = "Purchase Request" },
                new() { WfFormId = 2, FormName = "Customer Order" }
            },
            1,
            10,
            2
        );

        _wfFormRepoMock
            .Setup(x => x.WfFormSelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(wfForms);

        var result = await _sut.WfFormSelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow forms retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task WfFormSelectByIdAsync_WithValidId_ReturnsWfForm()
    {
        const int wfFormId = 1;
        var wfForm = new WfForm { WfFormId = wfFormId, FormName = "Purchase Request" };

        _wfFormRepoMock
            .Setup(x => x.WfFormSelectByIdAsync(wfFormId, _ct))
            .ReturnsAsync(wfForm);

        var result = await _sut.WfFormSelectByIdAsync(wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow form retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task WfFormInsertAsync_WithValidData_CommitsAndReturns201()
    {
        var wfFormVm = new WfFormVm { FormName = "Sales Invoice" };
        var insertedWfForm = new WfForm { WfFormId = 3, FormName = "Sales Invoice" };

        _wfFormRepoMock
            .Setup(x => x.WfFormInsertAsync(It.IsAny<WfForm>(), _ct))
            .ReturnsAsync(insertedWfForm);

        var result = await _sut.WfFormInsertAsync(wfFormVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow form created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task WfFormInsertAsync_SetsNoUserContext()
    {
        var wfFormVm = new WfFormVm { FormName = "Sales Invoice" };
        WfForm? capturedWfForm = null;

        _wfFormRepoMock
            .Setup(x => x.WfFormInsertAsync(It.IsAny<WfForm>(), _ct))
            .Callback<WfForm, CancellationToken>((f, _) => capturedWfForm = f)
            .ReturnsAsync(new WfForm { WfFormId = 1 });

        await _sut.WfFormInsertAsync(wfFormVm, _ct);

        Assert.NotNull(capturedWfForm);
        Assert.Equal(wfFormVm.FormName, capturedWfForm.FormName);
        _wfFormRepoMock.Verify(x => x.WfFormInsertAsync(It.IsAny<WfForm>(), _ct), Times.Once);
    }

    [Fact]
    public async Task WfFormUpdateAsync_WithValidData_CommitsTransaction()
    {
        var wfFormVm = new WfFormVm { WfFormId = 1, FormName = "Purchase Request Updated" };
        var updatedWfForm = new WfForm { WfFormId = 1, FormName = "Purchase Request Updated" };

        _wfFormRepoMock
            .Setup(x => x.WfFormUpdateAsync(It.IsAny<WfForm>(), _ct))
            .ReturnsAsync(updatedWfForm);

        var result = await _sut.WfFormUpdateAsync(wfFormVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow form updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task WfFormDeleteAsync_WithValidId_CommitsTransaction()
    {
        const int wfFormId = 1;

        _wfFormRepoMock
            .Setup(x => x.WfFormDeleteAsync(wfFormId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.WfFormDeleteAsync(wfFormId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow form deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task WfFormCboAsync_WithValidRequest_ReturnsForms()
    {
        var request = new CboRequestVm { Term = "Purchase" };
        var wfForms = new List<WfForm>
        {
            new() { WfFormId = 1, FormName = "Purchase Request" },
            new() { WfFormId = 2, FormName = "Purchase Order" }
        };

        _wfFormRepoMock
            .Setup(x => x.WfFormCboAsync(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(wfForms);

        var result = await _sut.WfFormCboAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Workflow forms for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task GetWorkflowConfigAsync_WithValidId_ReturnsConfigWithSteps()
    {
        const int wfFormId = 1;
        var wfForm = new WfForm { WfFormId = wfFormId, FormName = "Purchase Request" };
        var steps = new List<Wf>
        {
            new() { WfId = 10, WfFormId = wfFormId },
            new() { WfId = 11, WfFormId = wfFormId }
        };

        _wfFormRepoMock
            .Setup(x => x.WfFormSelectByIdAsync(wfFormId, _ct))
            .ReturnsAsync(wfForm);

        _wfRepoMock
            .Setup(x => x.WfSelectByWfFormIdAsync(wfFormId, _ct))
            .ReturnsAsync(steps);

        var result = await _sut.GetWorkflowConfigAsync(wfFormId, _ct);

        Assert.True(result.IsSuccess);
        var typed = Assert.IsType<ServiceResponse<WorkflowConfigVm>>(result);
        Assert.NotNull(typed.Data.Form);
        Assert.NotNull(typed.Data.Steps);
    }

    [Fact]
    public async Task WfFormInsertAsync_WhenRepoThrows_DoesNotCommit()
    {
        var wfFormVm = new WfFormVm { FormName = "Sales Invoice" };

        _wfFormRepoMock
            .Setup(x => x.WfFormInsertAsync(It.IsAny<WfForm>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.WfFormInsertAsync(wfFormVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
