using BankingService.Dal.Interfaces;
using BankingService.Sql.DbContext;

namespace BankingService.Dal.Services
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BankingServiceDbContext _dbContext;

        public IAccountRepository AccountRepository { get; }

        public IAccountTransactionRepository AccountTransactionRepository { get; }

        public IInterestRuleRepository InterestRuleRepository { get; }

        public UnitOfWork(BankingServiceDbContext dbContext,
            IAccountRepository accountRepository,
            IAccountTransactionRepository accountTransactionRepository,
            IInterestRuleRepository interestRuleRepository)
        {
            _dbContext = dbContext;            
            AccountRepository = accountRepository;
            AccountTransactionRepository = accountTransactionRepository;
            InterestRuleRepository = interestRuleRepository;
        }

        public async Task RunInTransaction(Func<Task> completeAction)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            await completeAction();
            await transaction.CommitAsync();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
