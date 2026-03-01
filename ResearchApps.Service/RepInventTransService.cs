using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class RepInventTransService : IRepInventTransService
{
    private readonly IRepInventTransRepo _repInventTransRepo;
    private readonly ILogger<RepInventTransService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public RepInventTransService(IRepInventTransRepo repInventTransRepo, ILogger<RepInventTransService> logger)
    {
        _repInventTransRepo = repInventTransRepo;
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "Generating InventTrans report for ItemId={ItemId}")]
    partial void LogGenerating(int itemId);

    [LoggerMessage(LogLevel.Error, "Error generating InventTrans report: {Message}")]
    partial void LogError(string message);

    public async Task<ServiceResponse<IEnumerable<RepInventTransByItemVm>>> RepInventTransByItem(int itemId, DateTime? startDate, DateTime? endDate, CancellationToken ct)
    {
        try
        {
            LogGenerating(itemId);
            var data = await _repInventTransRepo.RepInventTransByItem(itemId, startDate, endDate, ct);
            var result = _mapper.MapToVm(data);
            return ServiceResponse<IEnumerable<RepInventTransByItemVm>>.Success(result);
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
            return ServiceResponse<IEnumerable<RepInventTransByItemVm>>.Failure(ex.Message, 500);
        }
    }
}
