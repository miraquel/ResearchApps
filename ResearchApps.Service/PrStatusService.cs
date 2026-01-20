using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class PrStatusService : IPrStatusService
{
    private readonly IPrStatusRepo _prStatusRepo;
    private readonly ILogger<PrStatusService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public PrStatusService(IPrStatusRepo prStatusRepo, ILogger<PrStatusService> logger)
    {
        _prStatusRepo = prStatusRepo;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<PrStatusVm>>> PrStatusCboAsync(CboRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingPrStatuses();
        var repoResponse = await _prStatusRepo.PrStatusCboAsync(_mapper.MapToEntity(request), cancellationToken);
        LogPrStatusesRetrieved(repoResponse.Count());
        return ServiceResponse<IEnumerable<PrStatusVm>>.Success(_mapper.MapToVm(repoResponse), "PrStatus Cbo fetched successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving PR statuses for combo box")]
    partial void LogRetrievingPrStatuses();

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} PR statuses")]
    partial void LogPrStatusesRetrieved(int count);
}