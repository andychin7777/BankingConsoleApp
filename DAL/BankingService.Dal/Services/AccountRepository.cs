using BankingService.Dal.Interfaces;
using BankingService.Dal.Shared.Services;
using BankingService.Sql.BankingService.Model;
using BankingService.Sql.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BankingService.Dal.Services
{
    public class AccountRepository : GenericRepository<Account, int, AccountRepository>, IAccountRepository
    {
        public AccountRepository(BankingServiceDbContext context) : base(context)
        {
        }

        public async Task<Account?> GetByIdWithAccountTransactions(int id)
        {
            return await _dbSet.Include(x => x.AccountTransactions)
                .FirstAsync(x => x.AccountId == id);
        }
    }
}
