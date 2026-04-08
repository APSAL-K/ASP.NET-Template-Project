using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ModuleDrivenFramwork.Common.Modules;

public static class ModuleCollectionExtensions
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
    {
        var moduleTypes = GetModuleTypes(typeof(Program).Assembly);

        foreach (var moduleType in moduleTypes)
        {
            var module = (IAppModule)Activator.CreateInstance(moduleType)!;
            if (!ModuleConfiguration.IsEnabled(configuration, module.Name))
            {
                continue;
            }

            module.RegisterServices(services, configuration);
            services.AddSingleton(module);
        }

        return services;
    }

    public static IReadOnlySet<string> GetEnabledModuleNames(Assembly assembly, IConfiguration configuration)
    {
        return GetModuleTypes(assembly)
            .Select(type => (IAppModule)Activator.CreateInstance(type)!)
            .Where(module => ModuleConfiguration.IsEnabled(configuration, module.Name))
            .Select(module => module.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public static async Task InitializeApplicationModulesAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var modules = scope.ServiceProvider.GetServices<IAppModule>().OrderBy(module => module.Name, StringComparer.Ordinal);

        foreach (var module in modules)
        {
            await module.InitializeAsync(scope.ServiceProvider, app.Configuration, cancellationToken);
        }
    }

    private static Type[] GetModuleTypes(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(type => typeof(IAppModule).IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false })
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
    }
}