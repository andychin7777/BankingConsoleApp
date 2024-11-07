using BankingService.Bll.Model;

namespace BankingService.Bll.Helper
{
    internal static class AccountTransactionHelper
    {
        internal static decimal GetTotalSum(this List<AccountTransaction>? accountTransactions)
        {
            var total = accountTransactions?.Sum(x => x.Type == Shared.AccountTransactionType.Withdrawal ? -x.Amount : x.Amount);
            return total ?? 0;
        }
    }
}
