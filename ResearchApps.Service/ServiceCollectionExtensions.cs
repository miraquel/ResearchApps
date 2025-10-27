using Microsoft.Extensions.DependencyInjection;
using ResearchApps.Service.Interface;

namespace ResearchApps.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register service interfaces and their implementations
        services.AddScoped<IItemTypeService, ItemTypeService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IItemDeptService, ItemDeptService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IPrService, PrService>();
        services.AddScoped<IPrLineService, PrLineService>();
        
        return services;
    }
}