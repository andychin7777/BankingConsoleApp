using BankingService.Sql.BankingService.Model;
using Microsoft.EntityFrameworkCore;

namespace BankingService.Sql.DbContext
{
    public class BankingServiceDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public BankingServiceDbContext()
        {
        }

        public BankingServiceDbContext(DbContextOptions<BankingServiceDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountTransaction> AccountTransactions { get; set; }

        public virtual DbSet<InterestRule> InterestRules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
