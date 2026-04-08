using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApi.Common.Modules;
using MyApi.Modules.Auth.Application.Interfaces;
using MyApi.Modules.Auth.Persistence;
using MyApi.Modules.Auth.Services;

namespace MyApi.Modules.Auth;

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
        services.AddScoped<IAuthManagementService, AuthManagementService>();
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await using var dbContext = serviceProvider.GetRequiredService<AuthDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
