using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class PrStatusService : IPrStatusService
{
    private readonly IPrStatusRepo _prStatusRepo;
    private readonly MapperlyMapper _mapper = new();

    public PrStatusService(IPrStatusRepo prStatusRepo)
    {
        _prStatusRepo = prStatusRepo;
    }

    public async Task<ServiceResponse> PrStatusCboAsync(CboRequestVm request, CancellationToken cancellationToken)
    {
        var repoResponse = await _prStatusRepo.PrStatusCboAsync(_mapper.MapToEntity(request), cancellationToken);
        return ServiceResponse<IEnumerable<PrStatusVm>>.Success(_mapper.MapToVm(repoResponse), "PrStatus Cbo fetched successfully.");
    }
}