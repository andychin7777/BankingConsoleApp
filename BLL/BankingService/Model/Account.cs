namespace BankingService.Model;

public class Account
{
    public int AccountId { get; set; }
    public string? AccountName { get; set; }
    public IEnumerable<AccountTransaction>? AccountTransactions { get; set; }
}
