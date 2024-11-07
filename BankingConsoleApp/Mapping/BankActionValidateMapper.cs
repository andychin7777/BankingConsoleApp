using System.Text.RegularExpressions;
using BankingService.Bll.Model;
using Shared;
using Shared.Mapping;

namespace BankingConsoleApp.Mapping;

public static class BankActionValidateMapper
{
    private const string ValidationPrefixMessage = "Transaction Validation Message :";

    public static Notification<Account?> MapStringToAccountAndTransaction(string inputString)
    {
        if (String.IsNullOrEmpty(inputString))
        {
            return new Notification<Account?>
            {
                Success = false,
                Messages = new List<string> { $"{ValidationPrefixMessage} Blank input" }
            };
        }

        //<Date> <Account> <Type> <Amount>
        var split = inputString.Split(" ");
        if (split.Length != 4)
        {
            return new Notification<Account?>
            {
                Success = false,
                Messages = new List<string> { $"{ValidationPrefixMessage} Input string is invalid" }
            };
        }

        var returnNotification = new Notification<Account?>()
        {
            Success = true,
            Value = new Account()
            {
                AccountTransactions = new List<AccountTransaction>()
            }
        };
        //validate date is valid
        if (!DateOnly.TryParseExact(split[0], "yyyyMMdd", out var outDateOnly))
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{ValidationPrefixMessage} Date Input is an invalid Date");
        }

        //write account name into the account
        returnNotification.Value.AccountName = split[1];

        //try get the bank action type
        var mappingType = EnumnMapping.MapToAccountTransactionType(split[2]);
        if (mappingType == null)
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{ValidationPrefixMessage} Invalid Type");
        }

        //only allow x.00 or x.0 or x        
        var numberPartString = split[3];
        var decimalTryParse = Decimal.TryParse(numberPartString, out var outAmountValue);
        if (!Regex.IsMatch(numberPartString, @"^[0-9]*(\.[0-9]{1,2})?$")
            || !decimalTryParse)
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{ValidationPrefixMessage} Invalid Amount");
        }

        //map transaction only if there are no errors
        if (returnNotification.Success)
        {
            //continue to map the account transaction object in.
            returnNotification.Value.AccountTransactions.Add(new AccountTransaction()
            {
                Type = mappingType.Value,
                Date = outDateOnly,
                Amount = outAmountValue
            });
        }

        return returnNotification;
    }
}