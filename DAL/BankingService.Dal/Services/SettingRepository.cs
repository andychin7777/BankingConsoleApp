using BankingService.Dal.Interfaces;
using BankingService.Dal.Shared.Services;
using BankingService.Sql.DbContext;
using BankingService.Sql.OrgChart.Model;

namespace BankingService.Dal.Services
{
    public class SettingRepository : GenericRepository<Setting, string, SettingRepository>, ISettingRepository
    {
        public SettingRepository(BankingServiceDbContext context) : base(context)
        {
        }
    }
}
