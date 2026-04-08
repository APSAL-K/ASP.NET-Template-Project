using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MyApi.Common.Modules;

public sealed class ModuleControllerFeatureProvider : ControllerFeatureProvider
{
    private readonly IReadOnlySet<string> _enabledModules;

    public ModuleControllerFeatureProvider(IConfiguration configuration)
    {
        _enabledModules = ModuleCollectionExtensions.GetEnabledModuleNames(typeof(Program).Assembly, configuration);
    }

    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!base.IsController(typeInfo))
        {
            return false;
        }

        var moduleName = ModuleConfiguration.GetModuleName(typeInfo.AsType());
        if (moduleName == null)
        {
            return true;
        }

        return _enabledModules.Contains(moduleName);
    }
}



