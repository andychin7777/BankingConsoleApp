using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingService.Sql.BankingService.Model
{
    [Table("InterestRule")]
    public class InterestRule
    {
        [Key]
        public int InterestRuleId { get; set; }
        public string InterestRuleName { get; set; }
        public decimal InterestRate { get; set; }
        public string InterestRuleDateActive { get; set; }
    }
}
