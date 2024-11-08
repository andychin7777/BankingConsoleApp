using Shared;

namespace BankingService.Bll.Mapping;

public static class MapAccountTransaction
{
    public static Sql.Model.AccountTransaction? MapToSqlAccountTransaction(this Model.AccountTransaction accountTransaction)
    {
        if (accountTransaction == null)
        {
            return null;
        }
        return new Sql.Model.AccountTransaction()
        {
            AccountTransactionId = accountTransaction.AccountTransactionId,
            AccountId = accountTransaction.AccountId,
            Amount = accountTransaction.Amount,
            Date = accountTransaction.Date.ToString("yyyyMMdd"),
            Type = accountTransaction.Type.ToString()
        };
    }

    public static Model.AccountTransaction? MapToBllAccountTransaction(this Sql.Model.AccountTransaction accountTransaction)
    {
        if (accountTransaction == null)
        {
            return null;
        }
        return new Model.AccountTransaction()
        {
            AccountTransactionId = accountTransaction.AccountTransactionId,
            AccountId = accountTransaction.AccountId,
            Amount = accountTransaction.Amount,
            Date = DateOnly.ParseExact(accountTransaction.Date, "yyyyMMdd"),
            Type = Enum.Parse<AccountTransactionType>(accountTransaction.Type)
        };
    }
}
