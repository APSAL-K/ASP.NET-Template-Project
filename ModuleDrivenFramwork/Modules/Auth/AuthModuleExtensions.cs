using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModuleDrivenFramwork.Common.Modules;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;
using ModuleDrivenFramwork.Modules.Auth.Persistence;
using ModuleDrivenFramwork.Modules.Auth.Services;

namespace ModuleDrivenFramwork.Modules.Auth;

public sealed class AuthModule : IAppModule
{
    public string Name => "Auth";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = AuthDatabaseConfiguration.Resolve(configuration);

        services.AddDbContext<AuthDbContext>(options => AuthDatabaseConfiguration.Configure(options, databaseOptions));
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthUserStore, AuthUserStore>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthManagementService, IdentityManagementService>();
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await using var dbContext = serviceProvider.GetRequiredService<AuthDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
