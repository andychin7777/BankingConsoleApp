using BankingService.Dal.Shared.Interfaces;
using BankingService.Sql.Model;

namespace BankingService.Dal.Interfaces;

public interface IAccountTransactionRepository : IGenericRepository<AccountTransaction, int>
{
}