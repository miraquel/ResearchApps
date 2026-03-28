namespace ResearchApps.Service.Tests;

public class ReportServiceTests
{
    private readonly Mock<IReportRepo> _reportRepoMock;
    private readonly Mock<IReportParameterRepo> _reportParameterRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly ReportService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ReportServiceTests()
    {
        _reportRepoMock = new Mock<IReportRepo>();
        _reportParameterRepoMock = new Mock<IReportParameterRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        var loggerMock = new Mock<ILogger<ReportService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _sut = new ReportService(
            _reportRepoMock.Object,
            _reportParameterRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var reports = new PagedList<Report>(
            new List<Report>
            {
                new() { ReportId = 1, ReportName = "Report 1" },
                new() { ReportId = 2, ReportName = "Report 2" }
            },
            1,
            10,
            2
        );

        _reportRepoMock
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(reports);

        var result = await _sut.SelectAsync(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Reports retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsReportWithParameters()
    {
        var reportId = 1;
        var report = new Report { ReportId = reportId, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterId = 1, ReportId = reportId, ParameterName = "StartDate" }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(reportId, _ct))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(reportId, _ct))
            .ReturnsAsync(parameters);

        var result = await _sut.SelectByIdAsync(reportId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Report retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<ReportVm>>(result);
        var data = Assert.IsType<ReportVm>(typed.Data);
        Assert.NotNull(data);
        Assert.NotEmpty(data.Parameters);
    }

    [Fact]
    public async Task SelectByIdAsync_WithNonExistentId_ReturnsFailure()
    {
        var reportId = 999;

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(reportId, _ct))
            .ReturnsAsync((Report?)null);

        var result = await _sut.SelectByIdAsync(reportId, _ct);

        Assert.False(result.IsSuccess);
        Assert.Contains("Report not found.", result.Errors!);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task InsertAsync_WithValidReport_InsertsReportAndParameters()
    {
        var reportVm = new ReportVm
        {
            ReportName = "New Report",
            Parameters = [new() { ParameterName = "StartDate", IsRequired = true }]
        };
        var insertedReport = new Report { ReportId = 10, ReportName = "New Report" };

        _reportRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Report>(), _ct))
            .ReturnsAsync(insertedReport);
        _reportParameterRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ReportParameter>(), _ct))
            .ReturnsAsync(new ReportParameter());

        var result = await _sut.InsertAsync(reportVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Report inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _reportParameterRepoMock.Verify(
            x => x.InsertAsync(It.IsAny<ReportParameter>(), _ct),
            Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        var reportVm = new ReportVm { ReportName = "New Report", Parameters = [] };
        Report? capturedReport = null;

        _reportRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Report>(), _ct))
            .Callback<Report, CancellationToken>((r, _) => capturedReport = r)
            .ReturnsAsync(new Report { ReportId = 1 });

        await _sut.InsertAsync(reportVm, _ct);

        Assert.NotNull(capturedReport);
        Assert.Equal(_userClaimDto.Username, capturedReport.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidReport_UpdatesReportAndParameters()
    {
        var reportVm = new ReportVm
        {
            ReportId = 1,
            ReportName = "Updated Report",
            Parameters = [new() { ParameterName = "EndDate", IsRequired = true }]
        };
        var updatedReport = new Report { ReportId = 1, ReportName = "Updated Report" };

        _reportRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Report>(), _ct))
            .ReturnsAsync(updatedReport);
        _reportParameterRepoMock
            .Setup(x => x.DeleteByReportIdAsync(reportVm.ReportId, _ct))
            .Returns(Task.CompletedTask);
        _reportParameterRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ReportParameter>(), _ct))
            .ReturnsAsync(new ReportParameter());

        var result = await _sut.UpdateAsync(reportVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Report updated successfully.", result.Message);
        _reportParameterRepoMock.Verify(
            x => x.DeleteByReportIdAsync(reportVm.ReportId, _ct),
            Times.Once);
        _reportParameterRepoMock.Verify(
            x => x.InsertAsync(It.IsAny<ReportParameter>(), _ct),
            Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        var reportVm = new ReportVm
        {
            ReportId = 1,
            ReportName = "Updated Report",
            Parameters = []
        };
        Report? capturedReport = null;

        _reportRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Report>(), _ct))
            .Callback<Report, CancellationToken>((r, _) => capturedReport = r)
            .ReturnsAsync(new Report { ReportId = 1 });
        _reportParameterRepoMock
            .Setup(x => x.DeleteByReportIdAsync(It.IsAny<int>(), _ct))
            .Returns(Task.CompletedTask);

        await _sut.UpdateAsync(reportVm, _ct);

        Assert.NotNull(capturedReport);
        Assert.Equal(_userClaimDto.Username, capturedReport.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesReportAndParameters()
    {
        var reportId = 1;
        var modifiedBy = "admin";

        _reportParameterRepoMock
            .Setup(x => x.DeleteByReportIdAsync(reportId, _ct))
            .Returns(Task.CompletedTask);
        _reportRepoMock
            .Setup(x => x.DeleteAsync(reportId, modifiedBy, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(reportId, modifiedBy, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Report deleted successfully.", result.Message);
        _reportParameterRepoMock.Verify(
            x => x.DeleteByReportIdAsync(reportId, _ct),
            Times.Once);
        _reportRepoMock.Verify(
            x => x.DeleteAsync(reportId, modifiedBy, _ct),
            Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CboAsync_ReturnsReportList()
    {
        var reports = new List<Report>
        {
            new() { ReportId = 1, ReportName = "Report 1" },
            new() { ReportId = 2, ReportName = "Report 2" }
        };

        _reportRepoMock
            .Setup(x => x.CboAsync())
            .ReturnsAsync(reports);

        var result = await _sut.CboAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal("Reports for combo box retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task GetParametersAsync_WithValidReportId_ReturnsParameters()
    {
        var reportId = 1;
        var parameters = new List<ReportParameter>
        {
            new() { ParameterId = 1, ReportId = reportId, ParameterName = "StartDate" },
            new() { ParameterId = 2, ReportId = reportId, ParameterName = "EndDate" }
        };

        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(reportId, _ct))
            .ReturnsAsync(parameters);

        var result = await _sut.GetParametersAsync(reportId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Report parameters retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<ReportParameterVm>>>(result);
        var data = typed.Data!.ToList();
        Assert.NotNull(data);
        Assert.Equal(2, data.Count);
    }

    [Fact]
    public async Task GenerateReportAsync_WithValidParameters_ReturnsSuccess()
    {
        var generateVm = new ReportGenerateVm
        {
            ReportId = 1,
            ParameterValues = new Dictionary<string, string>
            {
                { "StartDate", "2026-01-01" },
                { "EndDate", "2026-01-31" }
            },
            OutputFormat = ReportOutputFormat.Pdf
        };
        var report = new Report { ReportId = 1, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterName = "StartDate", DisplayLabel = "Start Date", IsRequired = true },
            new() { ParameterName = "EndDate", DisplayLabel = "End Date", IsRequired = true }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, _ct))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(generateVm.ReportId, _ct))
            .ReturnsAsync(parameters);

        var result = await _sut.GenerateReportAsync(generateVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Report generation prepared successfully.", result.Message);
    }

    [Fact]
    public async Task GenerateReportAsync_WithNonExistentReport_ReturnsFailure()
    {
        var generateVm = new ReportGenerateVm { ReportId = 999 };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, _ct))
            .ReturnsAsync((Report?)null);

        var result = await _sut.GenerateReportAsync(generateVm, _ct);

        Assert.False(result.IsSuccess);
        Assert.Contains("Report not found.", result.Errors!);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GenerateReportAsync_WithMissingRequiredParameter_ReturnsFailure()
    {
        var generateVm = new ReportGenerateVm
        {
            ReportId = 1,
            ParameterValues = new Dictionary<string, string>()
        };
        var report = new Report { ReportId = 1, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterName = "StartDate", DisplayLabel = "Start Date", IsRequired = true }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, _ct))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(generateVm.ReportId, _ct))
            .ReturnsAsync(parameters);

        var result = await _sut.GenerateReportAsync(generateVm, _ct);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors!, e => e.Contains("Start Date") && e.Contains("required"));
    }

    [Fact]
    public async Task GenerateReportAsync_WithEmptyRequiredParameter_ReturnsFailure()
    {
        var generateVm = new ReportGenerateVm
        {
            ReportId = 1,
            ParameterValues = new Dictionary<string, string>
            {
                { "StartDate", "" }
            }
        };
        var report = new Report { ReportId = 1, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterName = "StartDate", DisplayLabel = "Start Date", IsRequired = true }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, _ct))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(generateVm.ReportId, _ct))
            .ReturnsAsync(parameters);

        var result = await _sut.GenerateReportAsync(generateVm, _ct);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors!, e => e.Contains("Start Date"));
    }

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var reportVm = new ReportVm { ReportName = "New Report", Parameters = [] };

        _reportRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Report>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(reportVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }
}
