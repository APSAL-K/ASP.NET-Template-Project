using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace MyApi.Modules.Auth.Persistence;

public static class AuthDatabaseConfiguration
{
    private const string DefaultConnectionString = "Data Source=Data/myapi.db";

    public static AuthDatabaseOptions Resolve(IConfiguration configuration, string? basePath = null)
    {
        var authDatabaseSection = configuration.GetSection("Modules:Auth:Database");
        var configuredProvider = authDatabaseSection["Provider"];
        var configuredConnectionString = authDatabaseSection["ConnectionString"];
        var configuredConnectionStringName = authDatabaseSection["ConnectionStringName"];

        var connectionString =
            configuredConnectionString ??
            configuration.GetConnectionString(configuredConnectionStringName ?? "AuthDb") ??
            configuration.GetConnectionString("AuthDb") ??
            DefaultConnectionString;

        var provider = NormalizeProvider(configuredProvider, connectionString);

        if (!string.Equals(provider, "sqlite", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(provider, "mysql", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported auth database provider '{provider}'. Configure Modules:Auth:Database:Provider as 'sqlite' or 'mysql'.");
        }

        if (string.Equals(provider, "sqlite", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(basePath))
        {
            connectionString = NormalizeSqliteConnectionString(connectionString, basePath);
        }

        return new AuthDatabaseOptions(provider, connectionString);
    }

    public static void Configure(DbContextOptionsBuilder optionsBuilder, AuthDatabaseOptions databaseOptions)
    {
        if (string.Equals(databaseOptions.Provider, "sqlite", StringComparison.OrdinalIgnoreCase))
        {
            EnsureSqliteDirectoryExists(databaseOptions.ConnectionString);
            optionsBuilder.UseSqlite(databaseOptions.ConnectionString);
            return;
        }

        if (string.Equals(databaseOptions.Provider, "mysql", StringComparison.OrdinalIgnoreCase))
        {
            var serverVersion = ServerVersion.AutoDetect(databaseOptions.ConnectionString);
            optionsBuilder.UseMySql(databaseOptions.ConnectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });
            return;
        }

        throw new InvalidOperationException($"Unsupported auth database provider '{databaseOptions.Provider}'.");
    }

    private static string NormalizeProvider(string? configuredProvider, string connectionString)
    {
        if (!string.IsNullOrWhiteSpace(configuredProvider))
        {
            return configuredProvider.Trim().ToLowerInvariant();
        }

        if (connectionString.Contains("server=", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("port=", StringComparison.OrdinalIgnoreCase))
        {
            return "mysql";
        }

        return connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
            ? "sqlite"
            : "mysql";
    }

    private static string NormalizeSqliteConnectionString(string connectionString, string basePath)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);

        if (!string.IsNullOrWhiteSpace(connectionStringBuilder.DataSource) && !Path.IsPathRooted(connectionStringBuilder.DataSource))
        {
            connectionStringBuilder.DataSource = Path.GetFullPath(Path.Combine(basePath, connectionStringBuilder.DataSource));
        }

        EnsureSqliteDirectoryExists(connectionStringBuilder.ConnectionString);
        return connectionStringBuilder.ConnectionString;
    }

    private static void EnsureSqliteDirectoryExists(string connectionString)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);
        var databaseDirectory = Path.GetDirectoryName(connectionStringBuilder.DataSource);

        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }
    }
}

public sealed record AuthDatabaseOptions(string Provider, string ConnectionString);