using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingService.Sql.Model;

[Table("Account")]
public class Account
{
    [Key]
    public int AccountId { get; set; }
    public string AccountName { get; set; }

    public ICollection<AccountTransaction> AccountTransactions { get; set; }
}