using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using Riok.Mapperly.Abstractions;

namespace ResearchApps.Mapper;

[Mapper]
public partial class MapperlyMapper
{
    // PagedListRequest
    public partial PagedListRequest MapToEntity(PagedListRequestVm pagedListRequestDto);
    public partial PagedListRequestVm MapToVm(PagedListRequest pagedListRequest);
    
    // ListRequest
    public partial ListRequest MapToEntity(ListRequestVm listRequestDto);
    public partial ListRequestVm MapToVm(ListRequest listRequest);
    
    // CboRequest
    public partial CboRequest MapToEntity(CboRequestVm cboRequestDto);
    public partial CboRequestVm MapToVm(CboRequest cboRequest);
    
    // PagedCboRequest
    public partial PagedCboRequest MapToEntity(PagedCboRequestVm pagedCboRequestDto);
    public partial PagedCboRequestVm MapToVm(PagedCboRequest pagedCboRequest);
    
    // ItemType
    public partial ItemType MapToEntity(ItemTypeVm itemTypeDto);
    public partial IEnumerable<ItemType> MapToEntity(IEnumerable<ItemTypeVm> itemTypeDto);
    public partial PagedList<ItemType> MapToEntity(PagedListVm<ItemTypeVm> pagedListDto);
    public partial ItemTypeVm MapToVm(ItemType itemType);
    public partial IEnumerable<ItemTypeVm> MapToVm(IEnumerable<ItemType> itemTypeDto);
    public partial PagedListVm<ItemTypeVm> MapToVm(PagedList<ItemType> pagedListDto);
    
    // Status
    public partial Status MapToEntity(StatusVm statusDto);
    public partial IEnumerable<StatusVm> MapToEntity(IEnumerable<StatusVm> statusDto);
    public partial PagedList<Status> MapToEntity(PagedListVm<StatusVm> pagedListDto);
    public partial StatusVm MapToVm(Status status);
    public partial IEnumerable<StatusVm> MapToVm(IEnumerable<Status> status);
    public partial PagedListVm<StatusVm> MapToVm(PagedList<Status> pagedListDto);
    
    // Item
    public partial Item MapToEntity(ItemVm itemDto);
    public partial IEnumerable<Item> MapToEntity(IEnumerable<ItemVm> itemDto);
    public partial PagedList<Item> MapToEntity(PagedListVm<ItemVm> pagedListDto);
    public partial ItemVm MapToVm(Item item);
    public partial IEnumerable<ItemVm> MapToVm(IEnumerable<Item> item);
    public partial PagedListVm<ItemVm> MapToVm(PagedList<Item> pagedListDto);
    
    // Warehouse
    public partial Warehouse MapToEntity(WarehouseVm warehouseDto);
    public partial IEnumerable<Warehouse> MapToEntity(IEnumerable<WarehouseVm> warehouseDto);
    public partial PagedList<Warehouse> MapToEntity(PagedListVm<WarehouseVm> pagedListDto);
    public partial WarehouseVm MapToVm(Warehouse warehouse);
    public partial IEnumerable<WarehouseVm> MapToVm(IEnumerable<Warehouse> warehouse);
    public partial PagedListVm<WarehouseVm> MapToVm(PagedList<Warehouse> pagedListDto);
    
    // ItemDept
    public partial ItemDept MapToEntity(ItemDeptVm itemDeptDto);
    public partial IEnumerable<ItemDept> MapToEntity(IEnumerable<ItemDeptVm> itemDeptDto);
    public partial PagedList<ItemDept> MapToEntity(PagedListVm<ItemDeptVm> pagedListDto);
    public partial ItemDeptVm MapToVm(ItemDept itemDept);
    public partial IEnumerable<ItemDeptVm> MapToVm(IEnumerable<ItemDept> itemDeptDto);
    public partial PagedListVm<ItemDeptVm> MapToVm(PagedList<ItemDept> pagedListDto);
    
    // Unit
    public partial Unit MapToEntity(UnitVm unitDto);
    public partial IEnumerable<Unit> MapToEntity(IEnumerable<UnitVm> unitDto);
    public partial PagedList<Unit> MapToEntity(PagedListVm<UnitVm> pagedListDto);
    public partial UnitVm MapToVm(Unit unit);
    public partial IEnumerable<UnitVm> MapToVm(IEnumerable<Unit> unitDto);
    public partial PagedListVm<UnitVm> MapToVm(PagedList<Unit> pagedListDto);
    
    // Pr
    public partial Pr MapToEntity(PrVm prDto);
    public partial IEnumerable<Pr> MapToEntity(IEnumerable<PrVm> prDto);
    public partial PagedList<Pr> MapToEntity(PagedListVm<PrVm> pagedListDto);
    public partial PrVm MapToVm(Pr pr);
    public partial IEnumerable<PrVm> MapToVm(IEnumerable<Pr> prDto);
    public partial PagedListVm<PrVm> MapToVm(PagedList<Pr> pagedListDto);
    
    // PrLine
    public partial PrLine MapToEntity(PrLineVm prLineDto);
    public partial IEnumerable<PrLine> MapToEntity(IEnumerable<PrLineVm> prLineDto);
    public partial PagedList<PrLine> MapToEntity(PagedListVm<PrLineVm> pagedListDto);
    public partial PrLineVm MapToVm(PrLine prLine);
    public partial IEnumerable<PrLineVm> MapToVm(IEnumerable<PrLine> prLineDto);
    public partial PagedListVm<PrLineVm> MapToVm(PagedList<PrLine> pagedListDto);
}