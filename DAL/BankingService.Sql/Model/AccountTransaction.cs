using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingService.Sql.Model;

[Table("AccountTransaction")]
public class AccountTransaction
{
    [Key]
    public int AccountTransactionId { get; set; }

    public int AccountId { get; set; }

    public string Date { get; set; }

    public string Type { get; set; }

    public decimal Amount { get; set; }
}
