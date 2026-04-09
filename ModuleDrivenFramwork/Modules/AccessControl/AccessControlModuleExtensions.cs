using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModuleDrivenFramwork.Common.Modules;
using ModuleDrivenFramwork.Modules.AccessControl.Persistence;
using ModuleDrivenFramwork.Modules.AccessControl.Services;

namespace ModuleDrivenFramwork.Modules.AccessControl;

public sealed class AccessControlModule : IAppModule
{
    public string Name => "AccessControl";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = AccessControlDatabaseConfiguration.Resolve(configuration);

        services.AddDbContext<AccessControlDbContext>(options => 
            AccessControlDatabaseConfiguration.Configure(options, databaseOptions));
            
        services.AddScoped<IAccessControlService, AccessControlService>();
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccessControlDbContext>();
        
        // Ensure the database and tables exist
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        // Seed basic data if missing
        if (!await dbContext.Roles.AnyAsync(cancellationToken))
        {
            await dbContext.Roles.AddRangeAsync(AccessControlSeedData.Roles, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!await dbContext.Permissions.AnyAsync(cancellationToken))
        {
            await dbContext.Permissions.AddRangeAsync(AccessControlSeedData.Permissions, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!await dbContext.RolePermissions.AnyAsync(cancellationToken))
        {
            await dbContext.RolePermissions.AddRangeAsync(AccessControlSeedData.RolePermissions, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
