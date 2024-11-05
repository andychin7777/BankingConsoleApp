using System;
using Shared;

namespace BankingService.Bll.Model;

public class AccountTransaction
{
    public int AccountTransactionId { get; set; }
    public int AccountId { get; set; }
    public DateOnly Date { get; set; }
    public BankActionType Type { get; set; }

    public decimal Amount {get;set;}
}