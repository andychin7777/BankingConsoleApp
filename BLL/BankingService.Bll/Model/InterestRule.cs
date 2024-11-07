namespace BankingService.Bll.Model
{
    public class InterestRule
    {
        public int InterestRuleId { get; set; }
        public string InterestRuleName { get; set; }
        public decimal InterestRate { get; set; }
        public DateOnly InterestRuleDateActive { get; set; }
    }
}
