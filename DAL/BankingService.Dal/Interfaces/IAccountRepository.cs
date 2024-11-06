using BankingService.Dal.Shared.Interfaces;
using BankingService.Sql.BankingService.Model;

namespace BankingService.Dal.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account, int>
    {
        Task<Account?> GetByIdWithAccountTransactions(int id);        
    }
}
