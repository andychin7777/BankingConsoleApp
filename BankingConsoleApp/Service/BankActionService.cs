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
                List<GroupedAccountTransaction> mappedResults = GetGroupedAccountTransactions(response);

                //|Date     | Txn Id      | Type | Amount
                Table table = new Table("Date", "Txn Id", "Type", "Amount");
                foreach (var groupItem in mappedResults)
                {
                    foreach (var accountTransaction in groupItem.RowCountGroup)
                    {
                        table.AddRow($"{accountTransaction.AccountTransaction.Date:yyyyMMdd}",
                            $"{accountTransaction.AccountTransaction.Date:yyyyMMdd}-{accountTransaction.RowCount:D0}",
                            accountTransaction.AccountTransaction.Type.MapToString(),
                            $"{accountTransaction.AccountTransaction.Amount:0.00}"
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

    private static List<GroupedAccountTransaction> GetGroupedAccountTransactions(Notification<Account> response)
    {
        return response.Value.AccountTransactions.OrderBy(x => x.Date)
                            .GroupBy(x => new { x.Date })
                            .Select(x => new GroupedAccountTransaction
                            {
                                RowCountGroup = x.OrderBy(x => x.AccountTransactionId).Select((accountTrans, i) => new AccountTransactionRowCountGroup
                                {
                                    RowCount = i + 1,
                                    AccountTransaction = accountTrans
                                }).ToList()
                            }).ToList();
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
            var response = await _bankingService.ProcessPrintStatement(mapperResponse.Value.Value.name, mapperResponse.Value.Value.startOfMonth);

            var groupedResponse = GetGroupedAccountTransactions(response);
            var dateOnlyForAccountPrint = mapperResponse.Value.Value.startOfMonth;

            if (response.Success)
            {
                //Date | Txn Id | Type | Amount | Balance
                Table table = new Table("Date", "Txn Id", "Type", "Amount", "Balance");

                foreach (var groupItem in groupedResponse)
                {
                    foreach (var rowCountGroupItem in groupItem.RowCountGroup)
                    {
                        if (rowCountGroupItem.AccountTransaction.Date.Year == dateOnlyForAccountPrint.Year && 
                            rowCountGroupItem.AccountTransaction.Date.Month == dateOnlyForAccountPrint.Month)
                        {
                            table.AddRow($"{rowCountGroupItem.AccountTransaction.Date:yyyyMMdd}",
                            rowCountGroupItem.AccountTransaction.Type == AccountTransactionType.Interest ? "" :
                                $"{rowCountGroupItem.AccountTransaction.Date:yyyyMMdd}-{rowCountGroupItem.RowCount:D0}",
                            rowCountGroupItem.AccountTransaction.Type.MapToString(),
                            $"{rowCountGroupItem.AccountTransaction.Amount:0.00}",
                            $"{rowCountGroupItem.AccountTransaction.Balance:0.00}");
                        }
                    }
                }
                 
                Console.WriteLine($"Account: {response.Value.AccountName}");
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

    private class GroupedAccountTransaction
    {
        public List<AccountTransactionRowCountGroup> RowCountGroup { get; set; }
    }

    private class AccountTransactionRowCountGroup
    {
        public int RowCount { get; set; }
        public AccountTransaction AccountTransaction { get; set; }
    }
}

