using BankingService.Bll.Model;
using Shared;

namespace BankingService.Bll.Mapping;

public static class MapAccountTransaction
{
    public static Sql.BankingService.Model.AccountTransaction? MapToSqlAccountTransaction(this AccountTransaction accountTransaction)
    {
        if (accountTransaction == null)
        {
            return null;
        }
        return new Sql.BankingService.Model.AccountTransaction()
        {
            AccountTransactionId = accountTransaction.AccountTransactionId,
            AccountId = accountTransaction.AccountId,
            Amount = accountTransaction.Amount,
            Date = accountTransaction.Date.ToString("yyyyMMdd"),
            Type = accountTransaction.Type.ToString()
        };
    }

    public static AccountTransaction? MapToBllAccountTransaction(this Sql.BankingService.Model.AccountTransaction accountTransaction)
    {
        if (accountTransaction == null)
        {
            return null;
        }
        return new AccountTransaction()
        {
            AccountTransactionId = accountTransaction.AccountTransactionId,
            AccountId = accountTransaction.AccountId,
            Amount = accountTransaction.Amount,
            Date = DateOnly.ParseExact(accountTransaction.Date, "yyyyMMdd"),
            Type = Enum.Parse<BankActionType>(accountTransaction.Type)
        };
    }
}
