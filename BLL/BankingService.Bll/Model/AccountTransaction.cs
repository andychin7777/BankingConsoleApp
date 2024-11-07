using System;
using Shared;

namespace BankingService.Bll.Model;

public class AccountTransaction
{
    public int AccountTransactionId { get; set; }
    public int AccountId { get; set; }
    public DateOnly Date { get; set; }
    public AccountTransactionType Type { get; set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// Field is not mapped from DAL layer in.
    /// </summary>
    public decimal Balance { get; set; }
}