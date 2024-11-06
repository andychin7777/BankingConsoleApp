using Microsoft.Extensions.DependencyInjection;
using BankingService.Dal.Interfaces;
using BankingService.Dal.Services;

namespace BankingService.Dal.Ioc
{
    public static class DalServiceCollectionExtension
    {
        public static IServiceCollection AddDal(this IServiceCollection services)
        {
            services.AddScoped<ISettingRepository, SettingRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
