namespace ResearchApps.Service.Tests;

/// <summary>
/// Unit tests for ReportService covering report management and generation
/// </summary>
public class ReportServiceTests
{
    private readonly Mock<IReportRepo> _reportRepoMock;
    private readonly Mock<IReportParameterRepo> _reportParameterRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<ReportService>> _loggerMock;
    private readonly ReportService _sut;

    public ReportServiceTests()
    {
        _reportRepoMock = new Mock<IReportRepo>();
        _reportParameterRepoMock = new Mock<IReportParameterRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<ReportService>>();

        _sut = new ReportService(
            _reportRepoMock.Object,
            _reportParameterRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    #region CRUD Tests

    [Fact]
    public async Task SelectAsync_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
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
            .Setup(x => x.SelectAsync(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(reports);

        // Act
        var result = await _sut.SelectAsync(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Reports retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task SelectByIdAsync_WithValidId_ReturnsReportWithParameters()
    {
        // Arrange
        var reportId = 1;
        var cancellationToken = CancellationToken.None;
        var report = new Report { ReportId = reportId, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterId = 1, ReportId = reportId, ParameterName = "StartDate" }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(reportId, cancellationToken))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(reportId, cancellationToken))
            .ReturnsAsync(parameters);

        // Act
        var result = await _sut.SelectByIdAsync(reportId, cancellationToken);

        // Assert
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
        // Arrange
        var reportId = 999;
        var cancellationToken = CancellationToken.None;

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(reportId, cancellationToken))
            .ReturnsAsync((Report?)null);

        // Act
        var result = await _sut.SelectByIdAsync(reportId, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Report not found.", result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task InsertAsync_WithValidReport_InsertsReportAndParameters()
    {
        // Arrange
        var reportVm = new ReportVm
        {
            ReportName = "New Report",
            Parameters = [new() { ParameterName = "StartDate", IsRequired = true }]
        };
        var cancellationToken = CancellationToken.None;
        var insertedReport = new Report { ReportId = 10, ReportName = "New Report" };

        _reportRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Report>(), cancellationToken))
            .ReturnsAsync(insertedReport);
        _reportParameterRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ReportParameter>(), cancellationToken))
            .ReturnsAsync(new ReportParameter());

        // Act
        var result = await _sut.InsertAsync(reportVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Report inserted successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _reportParameterRepoMock.Verify(
            x => x.InsertAsync(It.IsAny<ReportParameter>(), cancellationToken),
            Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var reportVm = new ReportVm { ReportName = "New Report", Parameters = [] };
        var cancellationToken = CancellationToken.None;
        Report? capturedReport = null;

        _reportRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Report>(), cancellationToken))
            .Callback<Report, CancellationToken>((r, _) => capturedReport = r)
            .ReturnsAsync(new Report { ReportId = 1 });

        // Act
        await _sut.InsertAsync(reportVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedReport);
        Assert.Equal(_userClaimDto.Username, capturedReport.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidReport_UpdatesReportAndParameters()
    {
        // Arrange
        var reportVm = new ReportVm
        {
            ReportId = 1,
            ReportName = "Updated Report",
            Parameters = [new() { ParameterName = "EndDate", IsRequired = true }]
        };
        var cancellationToken = CancellationToken.None;
        var updatedReport = new Report { ReportId = 1, ReportName = "Updated Report" };

        _reportRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Report>(), cancellationToken))
            .ReturnsAsync(updatedReport);
        _reportParameterRepoMock
            .Setup(x => x.DeleteByReportIdAsync(reportVm.ReportId, cancellationToken))
            .Returns(Task.CompletedTask);
        _reportParameterRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<ReportParameter>(), cancellationToken))
            .ReturnsAsync(new ReportParameter());

        // Act
        var result = await _sut.UpdateAsync(reportVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Report updated successfully.", result.Message);
        _reportParameterRepoMock.Verify(
            x => x.DeleteByReportIdAsync(reportVm.ReportId, cancellationToken),
            Times.Once);
        _reportParameterRepoMock.Verify(
            x => x.InsertAsync(It.IsAny<ReportParameter>(), cancellationToken),
            Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var reportVm = new ReportVm
        {
            ReportId = 1,
            ReportName = "Updated Report",
            Parameters = []
        };
        var cancellationToken = CancellationToken.None;
        Report? capturedReport = null;

        _reportRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Report>(), cancellationToken))
            .Callback<Report, CancellationToken>((r, _) => capturedReport = r)
            .ReturnsAsync(new Report { ReportId = 1 });
        _reportParameterRepoMock
            .Setup(x => x.DeleteByReportIdAsync(It.IsAny<int>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateAsync(reportVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedReport);
        Assert.Equal(_userClaimDto.Username, capturedReport.ModifiedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesReportAndParameters()
    {
        // Arrange
        var reportId = 1;
        var modifiedBy = "admin";
        var cancellationToken = CancellationToken.None;

        _reportParameterRepoMock
            .Setup(x => x.DeleteByReportIdAsync(reportId, cancellationToken))
            .Returns(Task.CompletedTask);
        _reportRepoMock
            .Setup(x => x.DeleteAsync(reportId, modifiedBy, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(reportId, modifiedBy, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Report deleted successfully.", result.Message);
        _reportParameterRepoMock.Verify(
            x => x.DeleteByReportIdAsync(reportId, cancellationToken),
            Times.Once);
        _reportRepoMock.Verify(
            x => x.DeleteAsync(reportId, modifiedBy, cancellationToken),
            Times.Once);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    #endregion

    #region ComboBox Tests

    [Fact]
    public async Task CboAsync_ReturnsReportList()
    {
        // Arrange
        var reports = new List<Report>
        {
            new() { ReportId = 1, ReportName = "Report 1" },
            new() { ReportId = 2, ReportName = "Report 2" }
        };

        _reportRepoMock
            .Setup(x => x.CboAsync())
            .ReturnsAsync(reports);

        // Act
        var result = await _sut.CboAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Reports for combo box retrieved successfully.", result.Message);
    }

    #endregion

    #region Parameter Tests

    [Fact]
    public async Task GetParametersAsync_WithValidReportId_ReturnsParameters()
    {
        // Arrange
        var reportId = 1;
        var cancellationToken = CancellationToken.None;
        var parameters = new List<ReportParameter>
        {
            new() { ParameterId = 1, ReportId = reportId, ParameterName = "StartDate" },
            new() { ParameterId = 2, ReportId = reportId, ParameterName = "EndDate" }
        };

        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(reportId, cancellationToken))
            .ReturnsAsync(parameters);

        // Act
        var result = await _sut.GetParametersAsync(reportId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Report parameters retrieved successfully.", result.Message);
        var typed = Assert.IsType<ServiceResponse<IEnumerable<ReportParameterVm>>>(result);
        var data = Assert.IsType<List<ReportParameterVm>>(typed.Data);
        Assert.NotNull(data);
        Assert.Equal(2, data.Count);
    }

    #endregion

    #region Report Generation Tests

    [Fact]
    public async Task GenerateReportAsync_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
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
        var cancellationToken = CancellationToken.None;
        var report = new Report { ReportId = 1, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterName = "StartDate", DisplayLabel = "Start Date", IsRequired = true },
            new() { ParameterName = "EndDate", DisplayLabel = "End Date", IsRequired = true }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, cancellationToken))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(generateVm.ReportId, cancellationToken))
            .ReturnsAsync(parameters);

        // Act
        var result = await _sut.GenerateReportAsync(generateVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Report generation prepared successfully.", result.Message);
    }

    [Fact]
    public async Task GenerateReportAsync_WithNonExistentReport_ReturnsFailure()
    {
        // Arrange
        var generateVm = new ReportGenerateVm { ReportId = 999 };
        var cancellationToken = CancellationToken.None;

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, cancellationToken))
            .ReturnsAsync((Report?)null);

        // Act
        var result = await _sut.GenerateReportAsync(generateVm, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Report not found.", result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GenerateReportAsync_WithMissingRequiredParameter_ReturnsFailure()
    {
        // Arrange
        var generateVm = new ReportGenerateVm
        {
            ReportId = 1,
            ParameterValues = new Dictionary<string, string>()
        };
        var cancellationToken = CancellationToken.None;
        var report = new Report { ReportId = 1, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterName = "StartDate", DisplayLabel = "Start Date", IsRequired = true }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, cancellationToken))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(generateVm.ReportId, cancellationToken))
            .ReturnsAsync(parameters);

        // Act
        var result = await _sut.GenerateReportAsync(generateVm, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Start Date", result.Message);
        Assert.Contains("required", result.Message);
    }

    [Fact]
    public async Task GenerateReportAsync_WithEmptyRequiredParameter_ReturnsFailure()
    {
        // Arrange
        var generateVm = new ReportGenerateVm
        {
            ReportId = 1,
            ParameterValues = new Dictionary<string, string>
            {
                { "StartDate", "" }
            }
        };
        var cancellationToken = CancellationToken.None;
        var report = new Report { ReportId = 1, ReportName = "Test Report" };
        var parameters = new List<ReportParameter>
        {
            new() { ParameterName = "StartDate", DisplayLabel = "Start Date", IsRequired = true }
        };

        _reportRepoMock
            .Setup(x => x.SelectByIdAsync(generateVm.ReportId, cancellationToken))
            .ReturnsAsync(report);
        _reportParameterRepoMock
            .Setup(x => x.SelectByReportIdAsync(generateVm.ReportId, cancellationToken))
            .ReturnsAsync(parameters);

        // Act
        var result = await _sut.GenerateReportAsync(generateVm, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Start Date", result.Message);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task InsertAsync_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var reportVm = new ReportVm { ReportName = "New Report", Parameters = [] };
        var cancellationToken = CancellationToken.None;

        _reportRepoMock
            .Setup(x => x.InsertAsync(It.IsAny<Report>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InsertAsync(reportVm, cancellationToken));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    #endregion
}





