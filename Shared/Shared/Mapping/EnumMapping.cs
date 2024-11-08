namespace Shared.Mapping;

public static class EnumMapping
{
    public static BankingActionType? MapToBankActionType(this string mappingString)
    {
        var toLowerString = mappingString.ToLowerInvariant();

        switch (toLowerString)
        {
            case "t":
                return BankingActionType.Transaction;
            case "i":
                return BankingActionType.InterestRules;
            case "p":
                return BankingActionType.PrintStatement;
            case "q":
                return BankingActionType.Quit;
            default:
                return null;
        }
    }

    public static AccountTransactionType? MapToAccountTransactionType(this string mappingString)
    {
        var toLowerString = mappingString.ToLowerInvariant();

        switch (toLowerString)
        {
            case "d":
                return AccountTransactionType.Deposit;
            case "w":
                return AccountTransactionType.Withdrawal;
            case "i":
                return AccountTransactionType.Interest;
            default:
                return null;
        }
    }

    public static string? MapToString(this AccountTransactionType accountTransactionType)
    {
        switch(accountTransactionType)
        {
            case AccountTransactionType.Deposit:
                return "D";
            case AccountTransactionType.Withdrawal:
                return "W";
            case AccountTransactionType.Interest:
                return "I";
            default: 
                return null;
        }
    }
}