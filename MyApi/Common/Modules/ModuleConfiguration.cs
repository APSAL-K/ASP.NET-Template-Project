using Microsoft.Extensions.Configuration;

namespace MyApi.Common.Modules;

public static class ModuleConfiguration
{
    public static bool IsEnabled(IConfiguration configuration, string moduleName)
    {
        var isEnabled = configuration.GetValue<bool?>("Modules:" + moduleName + ":Enabled");
        return isEnabled ?? true;
    }

    public static string? GetModuleName(Type type)
    {
        var namespaceValue = type.Namespace;
        if (string.IsNullOrWhiteSpace(namespaceValue))
        {
            return null;
        }

        var namespaceParts = namespaceValue.Split('.');
        for (var index = 0; index < namespaceParts.Length - 1; index++)
        {
            if (string.Equals(namespaceParts[index], "Modules", StringComparison.Ordinal) && index + 1 < namespaceParts.Length)
            {
                return namespaceParts[index + 1];
            }
        }

        return null;
    }
}