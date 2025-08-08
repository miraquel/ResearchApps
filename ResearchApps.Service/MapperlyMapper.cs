using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using Riok.Mapperly.Abstractions;

namespace ResearchApps.Service;

[Mapper]
public partial class MapperlyMapper
{
    // PagedListRequest
    public partial PagedListRequest MapToEntity(PagedListRequestVm pagedListRequestDto);
    public partial PagedListRequestVm MapToVm(PagedListRequest pagedListRequest);
    
    // ListRequest
    public partial ListRequest MapToEntity(ListRequestVm listRequestDto);
    public partial ListRequestVm MapToVm(ListRequest listRequest);
    
    // ItemType
    public partial ItemType MapToEntity(ItemTypeVm itemTypeDto);
    public partial IEnumerable<ItemType> MapToEntity(IEnumerable<ItemTypeVm> itemTypeDto);
    public partial ItemTypeVm MapToVm(ItemType itemType);
    public partial IEnumerable<ItemTypeVm> MapToVm(IEnumerable<ItemType> itemTypeDto);
    
    // Status
    public partial Status MapToEntity(StatusVm statusDto);
    public partial IEnumerable<StatusVm> MapToEntity(IEnumerable<StatusVm> statusDto);
    public partial StatusVm MapToVm(Status status);
    public partial IEnumerable<StatusVm> MapToVm(IEnumerable<Status> status);
    
}