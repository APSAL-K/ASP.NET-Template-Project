using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MyApi.Common.Modules;
using MyApi.Modules.Payment.Application.Interfaces;
using MyApi.Modules.Payment.Services;

namespace MyApi.Modules.Payment;

public sealed class PaymentModule : IAppModule
{
    public string Name => "Payment";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaymentService, PaymentService>();
    }
}
