using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
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

        return services;
    }
}