namespace BankingService.Bll.Mapping
{
    public static class MapInterestRule
    {
        public static Sql.Model.InterestRule? MapToSqlInterestRule(this Model.InterestRule interestRule)
        {
            if (interestRule == null)
            {
                return null;
            }
            return new Sql.Model.InterestRule()
            {
                InterestRuleId = interestRule.InterestRuleId,
                InterestRate = interestRule.InterestRate,
                InterestRuleDateActive = interestRule.InterestRuleDateActive.ToString("yyyyMMdd"),
                InterestRuleName = interestRule.InterestRuleName                
            };
        }

        public static Model.InterestRule? MapToBllInterestRule(this Sql.Model.InterestRule interestRule)
        {
            if (interestRule == null)
            {
                return null;
            }
            return new Model.InterestRule()
            {
                InterestRuleId = interestRule.InterestRuleId,
                InterestRate = interestRule.InterestRate,
                InterestRuleName = interestRule.InterestRuleName,
                InterestRuleDateActive = DateOnly.ParseExact(interestRule.InterestRuleDateActive, "yyyyMMdd")
            };
        }
    }    
}
