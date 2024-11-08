using System;
using BankingService.Dal.Interfaces;
using BankingService.Dal.Shared.Services;
using BankingService.Sql.DbContext;
using BankingService.Sql.Model;

namespace BankingService.Dal.Services;

public class AccountTransactionRepository : GenericRepository<AccountTransaction, int, AccountRepository>, IAccountTransactionRepository
{
    public AccountTransactionRepository(BankingServiceDbContext context) : base(context)
    {
    }
}

