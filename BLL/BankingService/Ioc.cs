using BankingService.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BankingService;

public static class Ioc
{
    public static void Init(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBankingService, Services.BankingService>();
    }
}
