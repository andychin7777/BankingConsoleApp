using BankingService.Dal.Interfaces;
using BankingService.Sql.DbContext;

namespace BankingService.Dal.Services
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BankingServiceDbContext _dbContext;

        public ISettingRepository SettingRepository { get; }

        public UnitOfWork(BankingServiceDbContext dbContext,
            ISettingRepository settingRepository)
        {
            _dbContext = dbContext;            
            SettingRepository = settingRepository;
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
