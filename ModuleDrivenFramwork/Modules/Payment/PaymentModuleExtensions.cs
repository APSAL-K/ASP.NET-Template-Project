using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ModuleDrivenFramwork.Common.Modules;
using ModuleDrivenFramwork.Modules.Payment.Application.Interfaces;
using ModuleDrivenFramwork.Modules.Payment.Services;

namespace ModuleDrivenFramwork.Modules.Payment;

public sealed class PaymentModule : IAppModule
{
    public string Name => "Payment";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaymentService, PaymentService>();
    }
}
