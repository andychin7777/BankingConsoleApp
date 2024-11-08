using BankingService.Dal.Interfaces;
using BankingService.Dal.Shared.Services;
using BankingService.Sql.DbContext;
using BankingService.Sql.Model;

namespace BankingService.Dal.Services
{
    public class InterestRuleRepository : GenericRepository<InterestRule, int, InterestRuleRepository>, IInterestRuleRepository
    {
        public InterestRuleRepository(BankingServiceDbContext context) : base(context)
        {
        }
    }
}
