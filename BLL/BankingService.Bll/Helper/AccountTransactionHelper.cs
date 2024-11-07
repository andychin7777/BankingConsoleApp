using BankingService.Bll.Model;

namespace BankingService.Bll.Helper
{
    internal static class AccountTransactionHelper
    {
        internal static decimal GetTotalSum(this List<AccountTransaction>? accountTransactions)
        {
            var total = accountTransactions?.Sum(x => x.GetTotalSum());
            return total ?? 0;
        }

        internal static decimal GetTotalSum(this AccountTransaction accountTransaction)
        {
            return accountTransaction.Type == Shared.AccountTransactionType.Withdrawal ? -accountTransaction.Amount : accountTransaction.Amount;
        }
    }
}
