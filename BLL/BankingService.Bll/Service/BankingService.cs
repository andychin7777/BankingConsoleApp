using BankingService.Bll.Helper;
using BankingService.Bll.Interface;
using BankingService.Bll.Mapping;
using BankingService.Bll.Model;
using BankingService.Dal.Interfaces;
using Shared;

namespace BankingService.Bll.Service;

public class BankingService : IBankingService
{
    private IUnitOfWork _unitOfWork { get; set; }

    public BankingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Notification<Account>> ProcessTransaction(Account toActionAccount)
    {
        var returnNotification = new Notification<Account>()
        {
            Success = true
        };
        var accountId = 0;

        //validate there are transactions to add
        if (!toActionAccount.AccountTransactions.Any())
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add("No transactions to add");
            return returnNotification;
        }

        await _unitOfWork.RunInTransaction(async () =>
        {
            //match exact on account name
            var databaseAccount = (await _unitOfWork.AccountRepository
                .FindWithAccountTransactions(x => x.AccountName == toActionAccount.AccountName, withTracking: false))
                .FirstOrDefault();
            var validateAccountTransactionCanRun = ValidateCanRunTransaction(databaseAccount, toActionAccount);

            if (!ValidateCanRunTransaction(databaseAccount, toActionAccount))
            {
                returnNotification.Success = false;
                returnNotification.Messages.Add("Unable to apply transactions as it would reduce the balance to below 0");
                return;
            }

            if (databaseAccount != null)
            {                
                //update the account transactions to have the respective ID value 
                toActionAccount.AccountTransactions.ForEach(x => x.AccountId = databaseAccount.AccountId);
                //append records to existing account
                await _unitOfWork.AccountTransactionRepository.AddRange(toActionAccount.AccountTransactions.Select(x => x.MapToSqlAccountTransaction()));                
            }
            else
            {
                //create new account
                databaseAccount = toActionAccount.MapToSqlAccount();
                await _unitOfWork.AccountRepository.Add(databaseAccount);
            }
            await _unitOfWork.SaveChanges();

            //get ID of new account
            accountId = databaseAccount.AccountId;
        });

        //check if there was any errors
        if (!returnNotification.Success)
        {
            return returnNotification;
        }

        //get list of transactions
        var newAccountResult = await _unitOfWork.AccountRepository.GetByIdWithAccountTransactions(accountId);
        returnNotification.Value = newAccountResult.MapToBllAccount();
        return returnNotification;
    }

    private bool ValidateCanRunTransaction(Sql.BankingService.Model.Account databaseAccount, Account currentToActionAccount)
    {
        var total = 0m;
        if (databaseAccount == null)
        {
            total = currentToActionAccount.GetTotalValueOfTransactions();
        }
        else
        {
            total = databaseAccount.MapToBllAccount().GetTotalValueOfTransactions() + currentToActionAccount.GetTotalValueOfTransactions();
        }        

        return total >= 0;
    }

    public async Task<Notification<List<InterestRule>>> ProcessDefineInterestRule(InterestRule interestRule)
    {
        var returnNotification = new Notification<List<InterestRule>>()
        {
            Success = true
        };

        await _unitOfWork.RunInTransaction(async () =>
        {
            var mappedIntoSqlInterestRule = interestRule.MapToSqlInterestRule();
            //find existing record match on date 
            var existingRecord = (await _unitOfWork.InterestRuleRepository.Find(x => x.InterestRuleDateActive == mappedIntoSqlInterestRule.InterestRuleDateActive))
                .FirstOrDefault();
            if (existingRecord != null)
            {
                //delete record
                await _unitOfWork.InterestRuleRepository.Delete(existingRecord.InterestRuleId);
                
            }
            await _unitOfWork.InterestRuleRepository.Add(interestRule.MapToSqlInterestRule());

            await _unitOfWork.SaveChanges();
            //get all records
            returnNotification.Value = (await _unitOfWork.InterestRuleRepository.All()).Select(x => x.MapToBllInterestRule())
                .ToList();
        });

        return returnNotification;
    }

    public async Task<Notification<Account>> ProcessPrintStatement(string accountName)
    {
        var result = (await _unitOfWork.AccountRepository.FindWithAccountTransactions(x => x.AccountName == accountName)).FirstOrDefault();
        if (result == null)
        {
            return new Notification<Account>()
            {
                Success = false,
                Messages = new List<string>()
                {
                    "Account does not exist"
                }
            };
        }
        var interestRates = await _unitOfWork.InterestRuleRepository.All();
        var account = result.MapToBllAccount();

        AddInterest(account, interestRates.Select(x => x.MapToBllInterestRule()).ToList());
        
        return new Notification<Account?>()
        {
            Success = true,
            Value = result.MapToBllAccount()
        };
    }

    /// <summary>
    /// function to add the interest rate records in per line.
    /// this is only done this one due to the specifications not being clear on when the 
    /// interest rate applies in what point in time and if it should be saved to the database
    /// due to time contraints this code is done as such below
    /// </summary>
    /// <param name="account"></param>
    internal void AddInterest(Account account, List<InterestRule> interestRules)
    {
        //start date
        var startDate = account.AccountTransactions.OrderBy(x => x.Date).FirstOrDefault();
        if (startDate == null)
        {
            //no interest to apply
            return;
        }

        var lastDate = account.AccountTransactions.OrderBy(x => x.Date).LastOrDefault();
        var _1MonthLater = lastDate.Date.AddMonths(1);

        var stopDate = new DateOnly(_1MonthLater.Year, _1MonthLater.Month, 01);
        
        InterestRule currentInterestRule = null;

        var currentDayMark = startDate.Date;

        var interestCounter = 0m;
        var totalRunningBalance = 0m;

        var dictionaryInterestRules = interestRules.ToDictionary(x => x.InterestRuleDateActive);
        var lookup = account.AccountTransactions.ToLookup(x => x.Date, x => x);
        //run through day by day
        while(currentDayMark <= stopDate)
        {
            //try find the current day balance.
            var currentDayTransactions = lookup[currentDayMark].OrderBy(x => x.AccountTransactionId).ToList();

            foreach(var transaction in currentDayTransactions)
            {
                totalRunningBalance = totalRunningBalance + transaction.GetTotalSum();
                transaction.Balance = totalRunningBalance;
            }

            if (dictionaryInterestRules.ContainsKey(currentDayMark))
            {
                currentInterestRule = dictionaryInterestRules[currentDayMark];
            }
            
            //apply the interest rule if it exists
            if (currentInterestRule != null)
            {
                //apply interest rule
                //treat year as 365 days 
                interestCounter = interestCounter + totalRunningBalance * currentInterestRule.InterestRate / 365;
            }

            //if day is end of the month, we need to apply interest and then apply it to the totalRunningBalance.
            if (currentDayMark.Month != currentDayMark.AddDays(1).Month)
            {
                var resultingRoundedInterest = Math.Round(interestCounter, 2, MidpointRounding.ToNegativeInfinity);
                account.AccountTransactions.Add(new AccountTransaction()
                {
                    Amount = resultingRoundedInterest,
                    Date = currentDayMark,
                    Type = AccountTransactionType.Interest,
                });
                //add to balance
                totalRunningBalance = totalRunningBalance + resultingRoundedInterest;

                //clear all interst
                interestCounter = 0m;
            }

            currentDayMark = currentDayMark.AddDays(1);
        }
    }
}
