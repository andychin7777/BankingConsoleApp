using BankingConsoleApp.Interface;
using BankingConsoleApp.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BankingConsoleApp;

public static class Ioc
{
    public static IServiceCollection AddProgram(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBankActionService, BankActionService>();        
        return serviceCollection;
    }
}
