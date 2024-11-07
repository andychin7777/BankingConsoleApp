using BankingService.Dal.Interfaces;
using BankingService.Dal.Shared.Services;
using BankingService.Sql.BankingService.Model;
using BankingService.Sql.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public async Task<IEnumerable<Account?>> FindWithAccountTransactions(Expression<Func<Account, bool>> predicate, bool withTracking = true)
        {
            return withTracking
                ? await _dbSet.Where(predicate).Include(x => x.AccountTransactions).ToListAsync()
                : await _dbSet.AsNoTracking().Where(predicate).Include(x => x.AccountTransactions).ToListAsync();
        }
    }
}
