using BankingService.Bll.Model;

namespace BankingService.Bll.Mapping
{
    public static class MapInterestRule
    {
        public static Sql.BankingService.Model.InterestRule? MapToSqlInterestRule(this InterestRule interestRule)
        {
            if (interestRule == null)
            {
                return null;
            }
            return new Sql.BankingService.Model.InterestRule()
            {
                InterestRuleId = interestRule.InterestRuleId,
                InterestRate = interestRule.InterestRate,
                InterestRuleDateActive = interestRule.InterestRuleDateActive.ToString("yyyyMMdd"),
                InterestRuleName = interestRule.InterestRuleName                
            };
        }

        public static InterestRule? MapToBllInterestRule(this Sql.BankingService.Model.InterestRule interestRule)
        {
            if (interestRule == null)
            {
                return null;
            }
            return new InterestRule()
            {
                InterestRuleId = interestRule.InterestRuleId,
                InterestRate = interestRule.InterestRate,
                InterestRuleName = interestRule.InterestRuleName,
                InterestRuleDateActive = DateOnly.ParseExact(interestRule.InterestRuleDateActive, "yyyyMMdd")
            };
        }
    }    
}
