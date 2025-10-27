using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ResearchApps.Domain;
using ResearchApps.Mapper;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Service
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepo _statusRepo;
        private readonly MapperlyMapper _mapper = new();
        public StatusService(IStatusRepo statusRepo)
        {
            _statusRepo = statusRepo;
        }
        public async Task<ServiceResponse> StatusCboAsync(CboRequestVm cboRequestVm, CancellationToken cancellationToken)
        {
            var statuses = await _statusRepo.StatusCboAsync(_mapper.MapToEntity(cboRequestVm), cancellationToken);
            var statusVms = _mapper.MapToVm(statuses);
            return ServiceResponse<IEnumerable<StatusVm>>.Success(statusVms,"Statuses retrieved successfully.", StatusCodes.Status200OK);
        }
    }
}
