using BankingService.Bll.Model;

namespace BankingService.Bll.Mapping;

public static class MapAccount
{
    public static Sql.BankingService.Model.Account? MapToSqlAccount(this Account account)
    {
        if (account == null)
        {
            return null;
        }
        return new Sql.BankingService.Model.Account()
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountTransactions = account.AccountTransactions?.Select(x => x.MapToSqlAccountTransaction()).ToList()
        };
    }

    public static Account? MapToBllAccount(this Sql.BankingService.Model.Account account)
    {
        if (account == null)
        {
            return null;
        }
        return new Account()
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountTransactions = account.AccountTransactions?.Select(x => x.MapToBllAccountTransaction()).ToList()
        };
    }
}
