namespace BankingService.Bll.Mapping;

public static class MapAccount
{
    public static Sql.Model.Account? MapToSqlAccount(this Model.Account account)
    {
        if (account == null)
        {
            return null;
        }
        return new Sql.Model.Account()
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountTransactions = account.AccountTransactions?.Select(x => x.MapToSqlAccountTransaction()).ToList()
        };
    }

    public static Model.Account? MapToBllAccount(this Sql.Model.Account account)
    {
        if (account == null)
        {
            return null;
        }
        return new Model.Account()
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountTransactions = account.AccountTransactions?.Select(x => x.MapToBllAccountTransaction()).ToList()
        };
    }
}
