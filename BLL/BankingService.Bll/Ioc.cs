using BankingService.Bll.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BankingService.Bll;

public static class Ioc
{
    public static IServiceCollection AddBll(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBankingService, BankingService.Bll.Service.BankingService>();

        return serviceCollection;
    }
}

