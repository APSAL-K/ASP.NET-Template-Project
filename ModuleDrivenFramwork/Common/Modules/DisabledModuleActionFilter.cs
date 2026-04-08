using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ModuleDrivenFramwork.Common.Modules;

public sealed class DisabledModuleActionFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public DisabledModuleActionFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controllerType = context.Controller.GetType();
        var moduleName = ModuleConfiguration.GetModuleName(controllerType);

        if (moduleName != null && !ModuleConfiguration.IsEnabled(_configuration, moduleName))
        {
            context.Result = new NotFoundResult();
            return Task.CompletedTask;
        }

        return next();
    }
}