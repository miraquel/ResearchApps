using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        // register IDbConnection
        services.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        });
        // register IDbTransaction
        services.AddScoped<IDbTransaction>(sp =>
        {
            var dbConnection = sp.GetRequiredService<IDbConnection>();
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            return dbConnection.BeginTransaction();
        });
        
        services.AddScoped<IItemTypeRepo, ItemTypeRepo>();
        services.AddScoped<IItemRepo, ItemRepo>();
        services.AddScoped<IWarehouseRepo, WarehouseRepo>();
        services.AddScoped<IItemDeptRepo, ItemDeptRepo>();
        services.AddScoped<IStatusRepo, StatusRepo>();
        services.AddScoped<IUnitRepo, UnitRepo>();
        services.AddScoped<IPrRepo, PrRepo>();
        services.AddScoped<IPrLineRepo, PrLineRepo>();
        services.AddScoped<IBudgetRepo, BudgetRepo>();
        services.AddScoped<IPrStatusRepo, PrStatusRepo>();
        services.AddScoped<IReportRepo, ReportRepo>();
        services.AddScoped<IReportParameterRepo, ReportParameterRepo>();
        services.AddScoped<INotificationRepo, NotificationRepo>();
    }
}