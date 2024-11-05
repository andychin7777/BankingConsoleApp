using System;
using BankingService.Bll.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BankingService.Bll;

public static class Ioc
{
    public static void Init(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBankingService, BankingService.Bll.Service.BankingService>();
    }
}

