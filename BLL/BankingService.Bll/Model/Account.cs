using BankingService.Bll.Helper;
using System;

namespace BankingService.Bll.Model;

public class Account
{
    public int AccountId { get; set; }
    public string? AccountName { get; set; }
    public List<AccountTransaction>? AccountTransactions { get; set; }

    public decimal GetTotalValueOfTransactions()
    {
        return AccountTransactions.GetTotalSum();
    }
}