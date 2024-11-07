using BankingConsoleApp.Interface;
using BankingConsoleApp.Mapping;
using BankingService.Bll.Interface;
using BankingService.Bll.Model;
using BetterConsoleTables;
using Microsoft.Extensions.Logging.Console;
using Shared;
using Shared.Mapping;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingConsoleApp.Service;

public class BankActionService : IBankActionService
{
    private IBankingService _bankingService;

    public BankActionService(IBankingService bankingService)
    {
        _bankingService = bankingService;
    }    

    public async Task<Notification> PerformTransaction(string inputString)
    {
        var mapperResponse = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);
        if (!mapperResponse.Success)
        {
            return mapperResponse;
        }

        try
        {
            var response = await _bankingService.ProcessTransaction(mapperResponse.Value);

            if (response.Success)
            {
                //requires grouping by date value for display purposes.
                var mappedResults = response.Value.AccountTransactions.OrderBy(x => x.Date)
                    .GroupBy(x => new { x.Date })
                    .Select(x => new                    
                    {
                        AccountTransactions = x.OrderBy(x => x.AccountTransactionId).Select((accountTrans, i) => new
                        {
                            RowCount = i + 1,
                            accountTrans
                        })
                    }).ToList();

                //|Date     | Txn Id      | Type | Amount
                Table  table = new Table("Date", "Txn Id", "Type", "Amount");
                foreach(var groupItem in mappedResults)
                {
                    foreach(var accountTransaction in groupItem.AccountTransactions)
                    {
                        table.AddRow($"{accountTransaction.accountTrans.Date:yyyyMMdd}", 
                            $"{accountTransaction.accountTrans.Date:yyyyMMdd}-{accountTransaction.RowCount:D0}",
                            accountTransaction.accountTrans.Type.MapToString(),
                            $"{accountTransaction.accountTrans.Amount:0.00}"
                        );
                    }
                }
                Console.WriteLine($"Account {response.Value.AccountName}:");
                Console.Write(table.ToString());
            }
            return response;
        }
        catch (Exception ex)
        {
            return new Notification
            {
                Success = false,
                Messages = new List<string> { ex.Message }
            };
        }
    }

    public async Task<Notification> PerformDefineInterestRules(string inputString)
    {
        var mapperResponse = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);
        if (!mapperResponse.Success)
        {
            return
                new Notification
                {
                    Success = mapperResponse.Success,
                    Messages = mapperResponse.Messages
                };
        }

        try
        {
            var response = await _bankingService.ProcessDefineInterestRule(mapperResponse.Value);
            if (response.Success)
            {
                //| Date | RuleId | Rate(%) |
                Table table = new Table("Date", "RuleId", "Rate (%)");
                foreach (var interestRule in response.Value)
                {
                    table.AddRow($"{interestRule.InterestRuleDateActive:yyyyMMdd}", interestRule.InterestRuleName, interestRule.InterestRate);
                }

                Console.WriteLine($"Interest Rules:");
                Console.Write(table.ToString());
            }

            return response;
        }
        catch (Exception ex)
        {
            return new Notification
            {
                Success = false,
                Messages = new List<string> { ex.Message }
            };
        }
    }

    public async Task<Notification> PerformPrintStatement(string inputString)
    {
        var mapperResponse = BankActionValidateMapper.MapStringToTupleNameAndStartOfMonth(inputString);
        if (!mapperResponse.Success)
        {
            return
                new Notification
                {
                    Success = mapperResponse.Success,
                    Messages = mapperResponse.Messages
                };
        }

        try
        {
            var response = await _bankingService.ProcessPrintStatement(mapperResponse.Value.Value.name);
            if (response.Success)
            {
                //Date | Txn Id | Type | Amount | Balance
                Table table = new Table("Date", "Txn Id", "Type", "Amount", "Balance");
                var currentBalance = 0m;
                foreach (var accountTransaction in response.Value.AccountTransactions)
                {
                    table.AddRow($"{accountTransaction.Date}",
                        accountTransaction.AccountTransactionId == 0 ? "" : accountTransaction.AccountTransactionId,


                        );
                }

                Console.WriteLine($"Account: {response.Value.AccountName}");
                Console.Write(table.ToString());
            }
        }
        catch (Exception ex)
        {
            return new Notification
            {
                Success = false,
                Messages = new List<string> { ex.Message }
            };
        }
    }

    private class GroupedAccountTransaction
    {
        public DateOnly Key { get; set; }

        public class AccountTransactionRowCountGroup
        {
            public int RowCount { get; set; }
            public AccountTransaction AccountTransaction { get; set; }
        }
    }
}

