using BankingService.Dal.Shared.Interfaces;
using BankingService.Sql.BankingService.Model;

namespace BankingService.Dal.Interfaces
{
    public interface IInterestRuleRepository : IGenericRepository<InterestRule, int>
    {
    }
}
