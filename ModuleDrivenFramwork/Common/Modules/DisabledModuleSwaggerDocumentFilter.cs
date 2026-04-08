using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModuleDrivenFramwork.Common.Modules;

public sealed class DisabledModuleSwaggerDocumentFilter : IDocumentFilter
{
    private readonly IConfiguration _configuration;

    public DisabledModuleSwaggerDocumentFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathsToRemove = context.ApiDescriptions
            .Where(description =>
            {
                var actionDescriptor = description.ActionDescriptor as ControllerActionDescriptor;
                var moduleName = actionDescriptor == null ? null : ModuleConfiguration.GetModuleName(actionDescriptor.ControllerTypeInfo.AsType());
                return moduleName != null && !ModuleConfiguration.IsEnabled(_configuration, moduleName);
            })
            .Select(description => "/" + (description.RelativePath ?? string.Empty).Trim('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var path in pathsToRemove)
        {
            swaggerDoc.Paths.Remove(path);
        }
    }
}