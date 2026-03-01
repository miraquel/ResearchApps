using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class RepCustomService : IRepCustomService
{
    private readonly IRepCustomRepo _repCustomRepo;
    private readonly ILogger<RepCustomService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public RepCustomService(IRepCustomRepo repCustomRepo, ILogger<RepCustomService> logger)
    {
        _repCustomRepo = repCustomRepo;
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "Generating Tools report for Year={Year}, Month={Month}")]
    partial void LogGeneratingTools(int year, int month);

    [LoggerMessage(LogLevel.Information, "Generating ToolsAnalysis report for Year={Year}, Month={Month}")]
    partial void LogGeneratingToolsAnalysis(int year, int month);

    [LoggerMessage(LogLevel.Error, "Error generating Custom report: {Message}")]
    partial void LogError(string message);

    public async Task<ServiceResponse<IEnumerable<RepToolsVm>>> RepTools(int year, int month, CancellationToken ct)
    {
        try
        {
            LogGeneratingTools(year, month);
            var data = await _repCustomRepo.RepTools(year, month, ct);
            var result = _mapper.MapToVm(data);
            return ServiceResponse<IEnumerable<RepToolsVm>>.Success(result);
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
            return ServiceResponse<IEnumerable<RepToolsVm>>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResponse<IEnumerable<RepToolsAnalysisVm>>> RepToolsAnalysis(int year, int month, CancellationToken ct)
    {
        try
        {
            LogGeneratingToolsAnalysis(year, month);
            var data = await _repCustomRepo.RepToolsAnalysis(year, month, ct);
            var result = _mapper.MapToVm(data);
            return ServiceResponse<IEnumerable<RepToolsAnalysisVm>>.Success(result);
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
            return ServiceResponse<IEnumerable<RepToolsAnalysisVm>>.Failure(ex.Message, 500);
        }
    }
}
