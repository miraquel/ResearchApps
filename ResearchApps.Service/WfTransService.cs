using System.Data;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class WfTransService : IWfTransService
{
    private readonly IWfTransRepo _wfTransRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly ILogger<WfTransService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public WfTransService(
        IWfTransRepo wfTransRepo,
        IDbTransaction dbTransaction,
        ILogger<WfTransService> logger)
    {
        _wfTransRepo = wfTransRepo;
        _dbTransaction = dbTransaction;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<WfTransHistoryVm>>> WfTransSelectByRefIdAsync(
        string refId, int wfFormId, CancellationToken cancellationToken)
    {
        LogRetrievingWfTrans(refId, wfFormId);
        var transactions = await _wfTransRepo.WfTransSelectByRefIdAsync(refId, wfFormId, cancellationToken);
        return ServiceResponse<IEnumerable<WfTransHistoryVm>>.Success(
            _mapper.MapToVm(transactions), "Workflow transactions retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Retrieving workflow transactions for RefId: {refId}, WfFormId: {wfFormId}")]
    partial void LogRetrievingWfTrans(string refId, int wfFormId);
}
