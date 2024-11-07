﻿using BankingService.Dal.Interfaces;
using BankingService.Dal.Shared.Services;
using BankingService.Sql.BankingService.Model;
using BankingService.Sql.DbContext;

namespace BankingService.Dal.Services
{
    public class InterestRuleRepository : GenericRepository<InterestRule, int, InterestRuleRepository>, IInterestRuleRepository
    {
        public InterestRuleRepository(BankingServiceDbContext context) : base(context)
        {
        }
    }
}