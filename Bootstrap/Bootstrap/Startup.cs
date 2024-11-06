﻿using BankingService.Bll;
using BankingService.Dal.Ioc;
using BankingService.Sql.Ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bootstrap;

public static class Startup
{
    private const string AppSettingsJson = "appsettings.json";
    public static void Init(IHostBuilder hostBuilder)
    {
        var builtConfig = new ConfigurationBuilder()
        .AddJsonFile(AppSettingsJson)
        .Build();

        hostBuilder
            .ConfigureAppConfiguration((hostingContext, config) => { config.AddConfiguration(builtConfig); })
            .ConfigureServices((hostingContext, services) =>
                    {
                        var configuration = hostingContext.Configuration;

                        services
                            .AddSql(configuration)
                            .AddDal()
                            .AddBll();
                    });
    }
}
