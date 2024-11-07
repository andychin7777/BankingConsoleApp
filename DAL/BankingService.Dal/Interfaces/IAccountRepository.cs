using BankingService.Dal.Shared.Interfaces;
using BankingService.Sql.BankingService.Model;
using System.Linq.Expressions;

namespace BankingService.Dal.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account, int>
    {
        Task<Account?> GetByIdWithAccountTransactions(int id);
        Task<IEnumerable<Account?>> FindWithAccountTransactions(Expression<Func<Account, bool>> predicate, bool withTracking = true);
    }
}
