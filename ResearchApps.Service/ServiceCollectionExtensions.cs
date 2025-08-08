using Microsoft.Extensions.DependencyInjection;
using ResearchApps.Service.Interface;

namespace ResearchApps.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register service interfaces and their implementations
        services.AddScoped<IItemTypeService, ItemTypeService>();
        
        return services;
    }
}