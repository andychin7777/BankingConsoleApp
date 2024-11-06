using BankingService.Dal.Shared.Interfaces;
using BankingService.Sql.OrgChart.Model;

namespace BankingService.Dal.Interfaces
{
    public interface ISettingRepository : IGenericRepository<Setting, string>
    {
    }
}
