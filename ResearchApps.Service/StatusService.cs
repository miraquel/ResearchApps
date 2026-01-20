using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Service
{
    public partial class StatusService : IStatusService
    {
        private readonly IStatusRepo _statusRepo;
        private readonly ILogger<StatusService> _logger;
        private readonly MapperlyMapper _mapper = new();
        public StatusService(IStatusRepo statusRepo, ILogger<StatusService> logger)
        {
            _statusRepo = statusRepo;
            _logger = logger;
        }
        public async Task<ServiceResponse<IEnumerable<StatusVm>>> StatusCboAsync(CboRequestVm cboRequestVm, CancellationToken cancellationToken)
        {
            LogRetrievingStatuses();
            var statuses = await _statusRepo.StatusCboAsync(_mapper.MapToEntity(cboRequestVm), cancellationToken);
            var statusVms = _mapper.MapToVm(statuses);
            LogStatusesRetrieved(statusVms.Count());
            return ServiceResponse<IEnumerable<StatusVm>>.Success(statusVms,"Statuses retrieved successfully.");
        }

        [LoggerMessage(LogLevel.Debug, "Retrieving statuses for combo box")]
        partial void LogRetrievingStatuses();

        [LoggerMessage(LogLevel.Debug, "Retrieved {count} statuses")]
        partial void LogStatusesRetrieved(int count);
    }
}
