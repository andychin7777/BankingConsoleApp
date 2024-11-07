using System.Data;
using System.Text.RegularExpressions;
using BankingService.Bll.Model;
using Shared;
using Shared.Mapping;

namespace BankingConsoleApp.Mapping;

public static class BankActionValidateMapper
{
    private const string TransactionValidationPrefixMessage = "Transaction Validation Message :";
    private const string DefineInterestRulePrefixMessage = "Interest Rule Validation Message :";

    public static Notification<Account?> MapStringToAccountAndTransaction(string inputString)
    {
        if (String.IsNullOrEmpty(inputString))
        {
            return new Notification<Account?>
            {
                Success = false,
                Messages = new List<string> { $"{TransactionValidationPrefixMessage} Blank input" }
            };
        }

        //<Date> <Account> <Type> <Amount>
        var split = inputString.Split(" ");
        if (split.Length != 4)
        {
            return new Notification<Account?>
            {
                Success = false,
                Messages = new List<string> { $"{TransactionValidationPrefixMessage} Input string is invalid" }
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
            returnNotification.Messages.Add($"{TransactionValidationPrefixMessage} Date Input is an invalid Date");
        }

        //write account name into the account
        returnNotification.Value.AccountName = split[1];

        //try get the bank action type
        var mappingType = EnumnMapping.MapToAccountTransactionType(split[2]);
        if (mappingType == null)
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{TransactionValidationPrefixMessage} Invalid Type");
        }

        //only allow x.00 or x.0 or x        
        var numberPartString = split[3];
        var decimalTryParse = Decimal.TryParse(numberPartString, out var outAmountValue);
        if (!Regex.IsMatch(numberPartString, @"^[0-9]*(\.[0-9]{1,2})?$")
            || !decimalTryParse)
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{TransactionValidationPrefixMessage} Invalid Amount");
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

    public static Notification<InterestRule?> MapStringToInterestRuleAndTransaction(string inputString)
    {
        if (String.IsNullOrEmpty(inputString))
        {
            return new Notification<InterestRule?>
            {
                Success = false,
                Messages = new List<string> { $"{DefineInterestRulePrefixMessage} Blank input" }
            };
        }

        //<Date> <RuleId> <Rate in %>
        var split = inputString.Split(" ");
        if (split.Length != 3)
        {
            return new Notification<InterestRule?>
            {
                Success = false,
                Messages = new List<string> { $"{DefineInterestRulePrefixMessage} Input string is invalid" },
                Value = new InterestRule()
            };
        }

        var returnNotification = new Notification<InterestRule?>()
        {
            Success = true
        };

        //validate date is valid
        if (!DateOnly.TryParseExact(split[0], "yyyyMMdd", out var outDateOnly))
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{DefineInterestRulePrefixMessage} Date Input is an invalid Date");
        }

        var interestRuleName = split[1];

        var numberPartString = split[2];
        var decimalTryParse = Decimal.TryParse(numberPartString, out var rate);        
        if (!Regex.IsMatch(numberPartString, @"^[0-9]*(\.[0-9]*)?$")
            || !decimalTryParse)
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{DefineInterestRulePrefixMessage} Invalid Rate");
        }

        if (!(rate > 0 && rate < 100))
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add($"{DefineInterestRulePrefixMessage} Rate must be between 0 and 100");
        }

        //map transaction only if there are no errors
        if (returnNotification.Success)
        {
            //continue to map the account transaction object in.
            returnNotification.Value = new InterestRule()
            {
                InterestRuleDateActive = outDateOnly,
                InterestRate = rate,
                InterestRuleName = interestRuleName
            };
        }

        return returnNotification;
    }
}