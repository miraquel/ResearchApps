using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class RepStockService : IRepStockService
{
    private readonly IRepInventTransRepo _repInventTransRepo;
    private readonly IRepStockRepo _repStockRepo;
    private readonly ILogger<RepStockService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public RepStockService(
        IRepInventTransRepo repInventTransRepo,
        IRepStockRepo repStockRepo,
        ILogger<RepStockService> logger)
    {
        _repInventTransRepo = repInventTransRepo;
        _repStockRepo = repStockRepo;
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "Generating InventTrans report for ItemId={ItemId}")]
    partial void LogGeneratingInventTrans(int itemId);

    [LoggerMessage(LogLevel.Information, "Generating StockCard report for ItemId={ItemId}, Year={Year}, Month={Month}")]
    partial void LogGeneratingStockCard(int itemId, int year, int month);

    [LoggerMessage(LogLevel.Error, "Error generating Stock report: {Message}")]
    partial void LogError(string message);

    public async Task<ServiceResponse<IEnumerable<RepInventTransByItemVm>>> RepInventTransByItem(int itemId, DateTime? startDate, DateTime? endDate, CancellationToken ct)
    {
        try
        {
            LogGeneratingInventTrans(itemId);
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

    public async Task<ServiceResponse<IEnumerable<RepStockCardMonthlyVm>>> RepStockCardMonthly(int itemId, int year, int month, CancellationToken ct)
    {
        try
        {
            LogGeneratingStockCard(itemId, year, month);
            var data = await _repStockRepo.RepStockCardMonthly(itemId, year, month, ct);
            var result = _mapper.MapToVm(data);
            return ServiceResponse<IEnumerable<RepStockCardMonthlyVm>>.Success(result);
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
            return ServiceResponse<IEnumerable<RepStockCardMonthlyVm>>.Failure(ex.Message, 500);
        }
    }
}
