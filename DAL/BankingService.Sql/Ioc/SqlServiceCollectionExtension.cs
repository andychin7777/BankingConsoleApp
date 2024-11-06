using BankingService.Sql.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankingService.Sql.Ioc
{
    public static class SqlServiceCollectionExtensions
    {
        public static IServiceCollection AddSql(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BankingServiceDbContext>(options => 
            {
                 options.UseSqlite(configuration.GetConnectionString("DbContext"));
            });
            return services;
        }
    }
}
