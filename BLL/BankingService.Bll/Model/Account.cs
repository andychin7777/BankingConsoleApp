using System;

namespace BankingService.Bll.Model;

public class Account
{
    public int AccountId { get; set; }
    public string? AccountName { get; set; }
    public List<AccountTransaction>? AccountTransactions { get; set; }

    public decimal GetTotalValueOfTransactions()
    {
        var total = AccountTransactions?.Sum(x => x.Type == Shared.AccountTransactionType.Withdrawal ? -x.Amount : x.Amount);
        return total ?? 0;
    }
}