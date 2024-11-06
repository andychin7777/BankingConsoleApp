using BankingConsoleApp.Interface;
using BankingConsoleApp.Mapping;
using BankingService.Bll.Interface;
using BankingService.Bll.Model;
using BetterConsoleTables;
using Shared;

namespace BankingConsoleApp.Service;

public class BankActionService : IBankActionService
{
    private IBankingService _bankingService;

    public BankActionService(IBankingService bankingService)
    {
        _bankingService = bankingService;
    }

    public async Task<Notification<Account?>> PerformTransaction(string inputString)
    {
        var bankActionHelperResponse = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);
        if (!bankActionHelperResponse.Success)
        {
            return bankActionHelperResponse;
        }

        try
        {
            var response = await _bankingService.ProcessTransaction(bankActionHelperResponse.Value);

            if (response.Success)
            {
                //requires grouping by date value for display purposes.
                var mappedResults = response.Value.AccountTransactions.OrderBy(x => x.Date)
                    .GroupBy(x => new { x.Date })
                    .Select(x => new
                    {
                        x.Key,
                        AccountTransaction = x.Select((accountTrans, i) => new
                        {
                            RowCount = i + 1,
                            accountTrans
                        })
                    }).ToList();

                //|Date     | Txn Id      | Type | Amount
                Table  table = new Table("Date", "Txn Id", "Type", "Amount");
                foreach(var groupItem in mappedResults)
                {
                    foreach(var accountTransaction in groupItem.AccountTransaction)
                    {
                        table.AddRow(accountTransaction.accountTrans.Date, 
                            $"{accountTransaction.accountTrans.Date:yyyyMMdd}-{accountTransaction.RowCount:D0}",
                            accountTransaction.accountTrans.Type.ToString(),
                            $"{accountTransaction.accountTrans.Amount:0.00}"
                        );
                    }
                }
                Console.Write(table.ToString());                
            }
            return response;
        }
        catch (Exception ex)
        {
            return new Notification<Account?>
            {
                Success = false,
                Messages = new List<string> { ex.Message }
            };
        }
    }
}

