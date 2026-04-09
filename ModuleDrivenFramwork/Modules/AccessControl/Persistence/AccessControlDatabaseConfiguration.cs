using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace ModuleDrivenFramwork.Modules.AccessControl.Persistence;

public static class AccessControlDatabaseConfiguration
{
    private const string DefaultConnectionString = "server=localhost;port=3306;database=myapi;user=root;password=root;";

    public static AccessControlDatabaseOptions Resolve(IConfiguration configuration)
    {
        var section = configuration.GetSection("Modules:AccessControl:Database");
        var configuredProvider = section["Provider"] ?? "mysql";
        var configuredConnectionString = section["ConnectionString"];
        var configuredConnectionStringName = section["ConnectionStringName"];

        var connectionString =
            configuredConnectionString ??
            configuration.GetConnectionString(configuredConnectionStringName ?? "Repository") ??
            configuration.GetConnectionString("Repository") ??
            DefaultConnectionString;

        return new AccessControlDatabaseOptions(configuredProvider, connectionString);
    }

    public static void Configure(DbContextOptionsBuilder optionsBuilder, AccessControlDatabaseOptions databaseOptions)
    {
        if (string.Equals(databaseOptions.Provider, "mysql", StringComparison.OrdinalIgnoreCase))
        {
            var serverVersion = ServerVersion.AutoDetect(databaseOptions.ConnectionString);
            optionsBuilder.UseMySql(databaseOptions.ConnectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });
            return;
        }

        // Fallback to sqlite if needed, but primary is mysql
        optionsBuilder.UseSqlite("Data Source=Data/modulerdrivenframwork.db");
    }
}

public sealed record AccessControlDatabaseOptions(string Provider, string ConnectionString);
