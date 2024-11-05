using BankingConsoleApp.Interface;
using BankingConsoleApp.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BankingConsoleApp;

public static class Ioc
{
    public static void Init(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBankActionService, BankActionService>();

        //load bll services
        BankingService.Bll.Ioc.Init(serviceCollection);
    }
}
