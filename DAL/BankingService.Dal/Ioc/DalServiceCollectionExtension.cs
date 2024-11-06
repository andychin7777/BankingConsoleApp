using Microsoft.Extensions.DependencyInjection;
using BankingService.Dal.Interfaces;
using BankingService.Dal.Services;

namespace BankingService.Dal.Ioc
{
    public static class DalServiceCollectionExtension
    {
        public static IServiceCollection AddDal(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountTransactionRepository, AccountTransactionRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
